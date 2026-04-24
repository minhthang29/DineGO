function acceptCookies() {
    document.cookie = "CookieConsent=true; path=/; max-age=" + 60 * 60 * 24 * 365;
    document.getElementById("cookie-consent-banner").remove();
}

function rejectCookies() {
    document.cookie = "CookieConsent=false; path=/; max-age=" + 60 * 60 * 24 * 365;
    document.getElementById("cookie-consent-banner").remove();
}

function reopenCookieBanner() {
    document.cookie = "CookieConsent=; path=/; max-age=0";
    location.reload();
}
