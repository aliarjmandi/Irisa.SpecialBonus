// نقطه مشترک همه درخواست‌های API
// چون فرانت روی همین سایت هاست شده، فقط از /api استفاده می‌کنیم
const API_BASE = "/api";

/**
 * فراخوانی عمومی API
 * @param {string} method - GET, POST, PUT, DELETE
 * @param {string} url - مثلا "/auth/login" یا "/admin/RewardPeriods"
 * @param {object|null} data - بدنه JSON
 * @param {function} onSuccess - تابع موفقیت
 * @param {function} onError - تابع خطا (اختیاری)
 */
function apiRequest(method, url, data, onSuccess, onError) {

    const headers = {
        "Content-Type": "application/json"
    };

    const token = localStorage.getItem("token");
    if (token) {
        headers["Authorization"] = "Bearer " + token;
    }

    $.ajax({
        method: method,
        url: API_BASE + url,
        data: data ? JSON.stringify(data) : null,
        headers: headers,
        success: onSuccess,
        error: function (xhr) {
            if (xhr.status === 401) {
                // توکن نامعتبر → برگرد به صفحه لاگین
                window.location.href = "/wwwroot/pages/login.html";
                return;
            }

            if (onError) {
                onError(xhr);
            } else {
                alert("خطا در ارتباط با سرور: " + (xhr.responseText || xhr.statusText));
            }
        }
    });
}
