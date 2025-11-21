// تنظیم نام کاربر در نوار بالا
function fillUserInfo() {
    const username = localStorage.getItem("username");
    if (username && $("#user").length) {
        $("#user").text(username);
    }
}

// خروج از سیستم
function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    localStorage.removeItem("role");
    window.location.href = "login.html";
}
