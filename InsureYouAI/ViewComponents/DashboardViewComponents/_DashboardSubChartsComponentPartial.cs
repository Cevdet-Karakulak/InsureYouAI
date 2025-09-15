using InsureYouAI.Context;
using InsureYouAI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InsureYouAI.ViewComponents.DashboardViewComponents
{
    public class _DashboardSubChartsComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
/*
 <div class="col">
    <div class="card rounded-4">
        <div class="card-body">
            <div class="d-flex align-items-center justify-content-between mb-3">
                <div class="">
                    <h4 class="mb-0">200</h4>
                    <p class="mb-0">Poliçe Türleri</p>
                </div>
                <div class="fs-2 text-facebook">
                    <i class="bi bi-facebook"></i>
                </div>
            </div>
            <div id="chart7"></div>
        </div>
    </div>
</div>
 */