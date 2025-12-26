/* ===================================================================
   HOMY ADMIN DASHBOARD - JavaScript
   Vanilla JS for all interactivity
   ================================================================== */

document.addEventListener('DOMContentLoaded', function () {
    // ===== DOM ELEMENTS =====
    const sidebar = document.getElementById('sidebar');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    const menuToggle = document.getElementById('menuToggle');
    const sidebarClose = document.getElementById('sidebarClose');
    const notificationBtn = document.getElementById('notificationBtn');
    const notificationMenu = document.getElementById('notificationMenu');
    const userDropdownBtn = document.getElementById('userDropdownBtn');
    const userDropdownMenu = document.getElementById('userDropdownMenu');
    const currentDateEl = document.getElementById('currentDate');
    const themeToggle = document.getElementById('themeToggle');

    // ===== THEME TOGGLE =====
    function initTheme() {
        const savedTheme = localStorage.getItem('homy-theme') || 'dark';
        document.documentElement.setAttribute('data-theme', savedTheme);
        document.documentElement.setAttribute('data-bs-theme', savedTheme); // Bootstrap 5.3 support
        updateThemeIcon(savedTheme);
    }

    function updateThemeIcon(theme) {
        if (themeToggle) {
            const icon = themeToggle.querySelector('i');
            if (icon) {
                icon.className = theme === 'dark' ? 'fas fa-moon' : 'fas fa-sun';
            }
        }
    }

    function toggleTheme() {
        const currentTheme = document.documentElement.getAttribute('data-theme') || 'dark';
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-theme', newTheme);
        document.documentElement.setAttribute('data-bs-theme', newTheme); // Bootstrap 5.3 support
        localStorage.setItem('homy-theme', newTheme);
        updateThemeIcon(newTheme);
        showToast(newTheme === 'dark' ? 'تم التبديل للوضع الداكن' : 'تم التبديل للوضع الفاتح', 'info');
    }

    if (themeToggle) {
        themeToggle.addEventListener('click', toggleTheme);
    }

    initTheme();

    // ===== SIDEBAR TOGGLE =====
    function openSidebar() {
        sidebar.classList.add('active');
        sidebarOverlay.classList.add('active');
        document.body.style.overflow = 'hidden';
    }

    function closeSidebar() {
        sidebar.classList.remove('active');
        sidebarOverlay.classList.remove('active');
        document.body.style.overflow = '';
    }

    // Menu toggle button click
    if (menuToggle) {
        menuToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            openSidebar();
        });
    }

    // Close button click
    if (sidebarClose) {
        sidebarClose.addEventListener('click', closeSidebar);
    }

    // Overlay click
    if (sidebarOverlay) {
        sidebarOverlay.addEventListener('click', closeSidebar);
    }

    // ===== DROPDOWN MENUS =====
    function closeAllDropdowns() {
        document.querySelectorAll('.dropdown-menu').forEach(function (menu) {
            menu.classList.remove('show');
        });
    }

    // Notification dropdown
    if (notificationBtn && notificationMenu) {
        notificationBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            const isOpen = notificationMenu.classList.contains('show');
            closeAllDropdowns();
            if (!isOpen) {
                notificationMenu.classList.add('show');
            }
        });
    }

    // User dropdown
    if (userDropdownBtn && userDropdownMenu) {
        userDropdownBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            const isOpen = userDropdownMenu.classList.contains('show');
            closeAllDropdowns();
            if (!isOpen) {
                userDropdownMenu.classList.add('show');
            }
        });
    }

    // Close dropdowns when clicking outside
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.nav-dropdown')) {
            closeAllDropdowns();
        }
    });

    // Close dropdowns on escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            closeAllDropdowns();
            closeSidebar();
        }
    });

    // ===== CURRENT DATE =====
    function updateCurrentDate() {
        const options = {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        };
        const date = new Date().toLocaleDateString('ar-EG', options);
        if (currentDateEl) {
            currentDateEl.textContent = date;
        }
    }

    updateCurrentDate();

    // ===== NAVIGATION ACTIVE STATE =====
    const navLinks = document.querySelectorAll('.nav-item .nav-link');

    navLinks.forEach(function (link) {
        link.addEventListener('click', function (e) {
            // Remove active class from all items
            document.querySelectorAll('.nav-item').forEach(function (item) {
                item.classList.remove('active');
            });

            // Add active class to clicked item's parent
            this.closest('.nav-item').classList.add('active');

            // Close sidebar on mobile after clicking
            if (window.innerWidth < 992) {
                closeSidebar();
            }
        });
    });

    // ===== STAT CARDS ANIMATION =====
    const statCards = document.querySelectorAll('.stat-card');

    // Animate numbers on page load
    statCards.forEach(function (card) {
        const valueElement = card.querySelector('.stat-value');
        if (valueElement) {
            const text = valueElement.textContent;
            const match = text.match(/[\d,]+/);
            if (match) {
                const targetValue = parseInt(match[0].replace(/,/g, ''));
                animateValue(valueElement, 0, targetValue, 1000, text);
            }
        }
    });

    function animateValue(element, start, end, duration, template) {
        const startTime = performance.now();

        function update(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);

            // Easing function
            const easeOutQuart = 1 - Math.pow(1 - progress, 4);
            const current = Math.floor(start + (end - start) * easeOutQuart);

            // Format with commas
            const formatted = current.toLocaleString('en-US');
            element.textContent = template.replace(/[\d,]+/, formatted);

            if (progress < 1) {
                requestAnimationFrame(update);
            }
        }

        requestAnimationFrame(update);
    }

    // ===== ACTION BUTTONS =====
    // View button click
    document.querySelectorAll('.btn-view').forEach(function (btn) {
        btn.addEventListener('click', function () {
            showToast('جاري عرض التفاصيل...', 'info');
        });
    });

    // Approve button click
    document.querySelectorAll('.btn-approve').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const row = this.closest('tr');
            const statusBadge = row.querySelector('.status-badge');
            if (statusBadge) {
                statusBadge.className = 'status-badge status-verified';
                statusBadge.textContent = 'موثق';
            }
            showToast('تمت الموافقة بنجاح!', 'success');
        });
    });

    // Reject button click
    document.querySelectorAll('.btn-reject').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const row = this.closest('tr');
            const statusBadge = row.querySelector('.status-badge');
            if (statusBadge) {
                statusBadge.className = 'status-badge status-rejected';
                statusBadge.textContent = 'مرفوض';
            }
            showToast('تم الرفض!', 'danger');
        });
    });

    // ===== QUICK ACTION BUTTONS =====
    document.querySelectorAll('.quick-action-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const actionText = this.querySelector('span').textContent;
            showToast('جاري تنفيذ: ' + actionText, 'info');
        });
    });

    // ===== TOAST NOTIFICATIONS =====
    function showToast(message, type) {
        // Create toast container if it doesn't exist
        let toastContainer = document.querySelector('.toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.className = 'toast-container';
            toastContainer.style.cssText = `
                position: fixed;
                top: 20px;
                left: 20px;
                z-index: 9999;
                display: flex;
                flex-direction: column;
                gap: 10px;
            `;
            document.body.appendChild(toastContainer);
        }

        // Create toast element
        const toast = document.createElement('div');
        toast.className = 'toast-notification';

        // Color based on type
        const colors = {
            success: '#10b981',
            danger: '#ef4444',
            warning: '#f59e0b',
            info: '#6366f1'
        };

        const icons = {
            success: 'fa-check-circle',
            danger: 'fa-times-circle',
            warning: 'fa-exclamation-circle',
            info: 'fa-info-circle'
        };

        toast.style.cssText = `
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 16px 20px;
            background: #1e293b;
            border-radius: 10px;
            box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
            border-right: 4px solid ${colors[type] || colors.info};
            color: #f8fafc;
            font-family: 'Cairo', sans-serif;
            font-size: 14px;
            transform: translateX(-100%);
            opacity: 0;
            transition: all 0.3s ease;
        `;

        toast.innerHTML = `
            <i class="fas ${icons[type] || icons.info}" style="color: ${colors[type] || colors.info}; font-size: 18px;"></i>
            <span>${message}</span>
        `;

        toastContainer.appendChild(toast);

        // Animate in
        requestAnimationFrame(function () {
            toast.style.transform = 'translateX(0)';
            toast.style.opacity = '1';
        });

        // Remove after 3 seconds
        setTimeout(function () {
            toast.style.transform = 'translateX(-100%)';
            toast.style.opacity = '0';
            setTimeout(function () {
                toast.remove();
            }, 300);
        }, 3000);
    }

    // ===== KEYBOARD SHORTCUTS =====
    document.addEventListener('keydown', function (e) {
        // Ctrl/Cmd + K for search focus
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            const searchInput = document.querySelector('.search-input');
            if (searchInput) {
                searchInput.focus();
            }
        }
    });

    // ===== WINDOW RESIZE HANDLER =====
    let resizeTimer;
    window.addEventListener('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            // Close sidebar if window is resized to desktop
            if (window.innerWidth >= 992) {
                closeSidebar();
            }
        }, 250);
    });

    // ===== INITIALIZE TOOLTIPS (if Bootstrap tooltips needed) =====
    // Bootstrap tooltips initialization
    const tooltipTriggerList = document.querySelectorAll('[title]');
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        // Simple native tooltip - Bootstrap handles this automatically
    });

    console.log('🏠 هومي - لوحة التحكم تم تحميلها بنجاح!');
});
