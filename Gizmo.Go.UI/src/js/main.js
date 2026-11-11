// Gizmo Go - main JS entry point.
import '../scss/app.scss';

window.gizmoGoLifecycle = {
    // Комбинированный детектор возврата пользователя из внешнего приложения (Telegram).
    //
    // Два механизма дополняют друг друга:
    //   - visibilitychange: работает на мобильном браузере — когда браузер уходит
    //     в фон целиком (document.hidden = true), например при открытии Telegram.
    //   - window blur/focus: работает на десктопе — когда другое приложение встаёт
    //     поверх браузерного окна, document.hidden остаётся false, но window.blur
    //     срабатывает корректно.
    //
    // Оба источника пишут в один _hiddenAt и читают из него onShow, что исключает
    // двойной вызов OnResumed: первый сработавший event сбрасывает _hiddenAt в null.
    //
    // startWatching() сбрасывает _hiddenAt до нуля — вызывается со страницы ожидания
    // после подписки на Resumed, чтобы аннулировать stale blur от window.open("tg://..."),
    // который произошёл до загрузки страницы.
    _hiddenAt: null,
    _MIN_AWAY_MS: 300,
    register: function(dotNetRef) {
        var lc = window.gizmoGoLifecycle;

        function onHide() {
            if (lc._hiddenAt === null) {
                lc._hiddenAt = Date.now();
            }
        }

        function onShow() {
            if (lc._hiddenAt === null) return;
            var elapsed = Date.now() - lc._hiddenAt;
            lc._hiddenAt = null;
            if (elapsed >= lc._MIN_AWAY_MS) {
                dotNetRef.invokeMethodAsync('OnResumed');
            }
        }

        document.addEventListener('visibilitychange', function() {
            if (document.hidden) onHide(); else onShow();
        });
        window.addEventListener('blur', onHide);
        window.addEventListener('focus', onShow);
    },
    isActive: function() {
        return !document.hidden && document.hasFocus();
    },
    startWatching: function() {
        var lc = window.gizmoGoLifecycle;
        // Если браузер уже без фокуса (Telegram уже на переднем плане) —
        // начинаем отсчёт прямо сейчас, чтобы не потерять текущий "ушёл" цикл.
        // Иначе сбрасываем stale состояние и ждём нового blur.
        if (document.hidden || !document.hasFocus()) {
            lc._hiddenAt = Date.now();
        } else {
            lc._hiddenAt = null;
        }
    },
    _placeholderWindow: null,
    openPlaceholder: function() {
        try {
            var w = window.open('about:blank', '_blank');
            if (w) { window.gizmoGoLifecycle._placeholderWindow = w; return true; }
            return false;
        } catch { return false; }
    },
    redirectPlaceholder: function(url) {
        var w = window.gizmoGoLifecycle._placeholderWindow;
        if (w && !w.closed) w.location.href = url;
        window.gizmoGoLifecycle._placeholderWindow = null;
    },
    closePlaceholder: function() {
        var lc = window.gizmoGoLifecycle;
        if (lc._placeholderWindow && !lc._placeholderWindow.closed) {
            try { lc._placeholderWindow.close(); } catch {}
        }
        lc._placeholderWindow = null;
    },
    triggerProtocol: function(url) {
        var a = document.createElement('a');
        a.href = url;
        a.target = '_self';
        a.style.display = 'none';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    }
};

window.gizmoGoScroll = {
    // Scrolls a horizontal track back to its start (the news carousel when a new first news arrives).
    toStart: function (element) {
        if (!element) return;
        element.scrollTo({ left: 0, behavior: 'smooth' });
    },

    // Centers an element of the page and briefly marks it with a class (a reservation opened from a notification);
    // the class is removed when its animation ends, so the same element can be marked again.
    toElement: function (id, highlightClass) {
        var element = document.getElementById(id);
        if (!element) return;

        element.scrollIntoView({ block: 'center', behavior: 'smooth' });

        if (highlightClass) {
            element.classList.remove(highlightClass);
            void element.offsetWidth;
            element.classList.add(highlightClass);
            element.addEventListener('animationend', function () {
                element.classList.remove(highlightClass);
            }, { once: true });
        }
    }
};
