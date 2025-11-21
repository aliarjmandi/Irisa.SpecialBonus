// تابعی برای بررسی لاگین بودن کاربر در صفحات محافظت‌شده
function requireAuth() {
    const token = localStorage.getItem("token");
    if (!token) {
        window.location.href = "login.html";
        return false;
    }
    return true;
}

// مخصوص صفحه لاگین
$(document).ready(function () {

    // اگر قبلا لاگین کرده، مستقیم ببرد روی داشبورد
    if (localStorage.getItem("token")) {
        // اگر روی login.html هستیم
        if (window.location.pathname.toLowerCase().includes("login.html")) {
            window.location.href = "dashboard.html";
            return;
        }
    }

    $("#btnLogin").on("click", function () {
        const req = {
            userNameOrEmail: $("#username").val(),
            password: $("#password").val()
        };

        apiRequest("POST", "/Auth/login", req, function (res) {
            // فرض: پاسخ شامل token, username, role است
            localStorage.setItem("token", res.token);
            localStorage.setItem("username", res.username);
            localStorage.setItem("role", res.role);

            window.location.href = "dashboard.html";

        }, function (xhr) {
            let msg = "ورود ناموفق بود.";
            if (xhr.responseJSON && xhr.responseJSON.message)
                msg = xhr.responseJSON.message;
            else if (xhr.responseText)
                msg = xhr.responseText;

            $("#error").text(msg).show();
        });
    });
});
