using Homy.Application.Contract_Service.ApiServices;
using Homy.Application.Dtos.ApiDtos;
using Homy.Domin.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Homy.presentaion.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PropertyApprovalController : Controller
    {
        private readonly IPropertyApiService _propertyService;

        public PropertyApprovalController(IPropertyApiService propertyService)
        {
            _propertyService = propertyService;
        }

        // GET: PropertyApproval
        public async Task<IActionResult> Index(string status = "Pending")
        {
            try
            {
                var filter = new PropertyFilterDto
                {
                    Status = status switch
                    {
                        "Pending" => (int)PropertyStatus.PendingReview,
                        "Active" => (int)PropertyStatus.Active,
                        "Rejected" => (int)PropertyStatus.Rejected,
                        _ => (int)PropertyStatus.PendingReview
                    },
                    PageSize = 100,
                    PageNumber = 1
                };

                var properties = await _propertyService.GetPropertiesAsync(filter);
                ViewBag.CurrentStatus = status;
                return View(properties);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
                return View();
            }
        }

        // GET: PropertyApproval/Details/5
        public async Task<IActionResult> Details(long id)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(id);
                
                if (property == null)
                {
                    TempData["Error"] = "الإعلان غير موجود";
                    return RedirectToAction(nameof(Index));
                }

                return View(property);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PropertyApproval/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id)
        {
            try
            {
                var dto = new PropertyApprovalDto
                {
                    PropertyId = id,
                    IsApproved = true
                };

                var (success, message) = await _propertyService.ApproveOrRejectPropertyAsync(dto);

                if (success)
                    TempData["Success"] = message;
                else
                    TempData["Error"] = message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: PropertyApproval/Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    TempData["Error"] = "يجب كتابة سبب الرفض";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var dto = new PropertyApprovalDto
                {
                    PropertyId = id,
                    IsApproved = false,
                    RejectionReason = reason
                };

                var (success, message) = await _propertyService.ApproveOrRejectPropertyAsync(dto);

                if (success)
                    TempData["Success"] = message;
                else
                    TempData["Error"] = message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
