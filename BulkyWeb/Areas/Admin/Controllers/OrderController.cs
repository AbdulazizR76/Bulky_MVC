﻿
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;


namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
           
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product"),
            };
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;


            if (OrderVM.OrderHeader.Carrier != null)
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details),new {orderId= orderHeaderFromDb.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcessing);
            _unitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderheaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

            orderheaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderheaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            orderheaderFromDb.OrderStatus = SD.StatusShipped;
            orderheaderFromDb.ShippingDate = DateTime.Now;
            if (orderheaderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderheaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.OrderHeader.Update(orderheaderFromDb);
            _unitOfWork.Save();

            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusShipped);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderheaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderheaderFromDb.PaymentStatus == SD.PaymentStatusApproved)
            {
                //Refund the money
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderheaderFromDb.PaymentIntentId,

                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStatus(orderheaderFromDb.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderheaderFromDb.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [ActionName("Details")]
        [HttpPost]
        public IActionResult Details_PAY_NOW()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            // strip logic 
            var domain = Request.Scheme+ "://"+ Request.Host.Value+"/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = (long)(item.Price * 100), // $20.50 => 2050 cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        }
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }
            // if this don't work then use the below code
            var service = new Stripe.Checkout.SessionService();
            //var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentStatus);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //this is an order by a company 

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
              

               
            }
            return View(orderHeaderId);
        }

        #region API CALLS

        [HttpGet]
        
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders= _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId,includeProperties: "ApplicationUser");
            }

                switch (status)
                {
                    case "pending":
                        objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending).ToList();
                        break;
                    case "inproccess":
                        objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcessing).ToList();
                        break;
                    case "completed":
                        objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped).ToList();
                        break;
                    case "approved":
                        objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved).ToList();
                        break;
                    default:
                        break;
                }
            return Json(new { data = objOrderHeaders });
        }

        
        #endregion
    }
}
