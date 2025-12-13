/* ===================================================================
   DASHBOARD CHARTS - Chart.js Configuration
   Interactive Donut Charts for Data Visualization
   ================================================================== */

document.addEventListener('DOMContentLoaded', function() {
    // Chart.js default configuration for RTL and dark theme
    Chart.defaults.font.family = "'Cairo', sans-serif";
    Chart.defaults.color = '#94a3b8';
    
    // ===== USERS DISTRIBUTION CHART =====
    const usersChartCanvas = document.getElementById('usersChart');
    if (usersChartCanvas) {
        const usersChart = new Chart(usersChartCanvas, {
            type: 'doughnut',
            data: {
                labels: ['مستخدمين نشطين', 'مستخدمين جدد', 'غير نشطين'],
                datasets: [{
                    data: [65, 25, 10],
                    backgroundColor: [
                        '#6366f1',
                        '#8b5cf6',
                        '#ec4899'
                    ],
                    borderWidth: 0,
                    hoverOffset: 10
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: '#1e293b',
                        titleColor: '#f8fafc',
                        bodyColor: '#f8fafc',
                        borderColor: '#334155',
                        borderWidth: 1,
                        padding: 12,
                        cornerRadius: 8,
                        displayColors: true,
                        callbacks: {
                            label: function(context) {
                                return context.label + ': ' + context.parsed + '%';
                            }
                        }
                    }
                },
                animation: {
                    animateRotate: true,
                    animateScale: true,
                    duration: 1500,
                    easing: 'easeOutQuart'
                }
            }
        });
    }

    // ===== ADS DISTRIBUTION CHART =====
    const adsChartCanvas = document.getElementById('adsChart');
    if (adsChartCanvas) {
        const adsChart = new Chart(adsChartCanvas, {
            type: 'doughnut',
            data: {
                labels: ['إعلانات نشطة', 'قيد المراجعة', 'منتهية'],
                datasets: [{
                    data: [70, 20, 10],
                    backgroundColor: [
                        '#10b981',
                        '#f59e0b',
                        '#ef4444'
                    ],
                    borderWidth: 0,
                    hoverOffset: 10
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: '#1e293b',
                        titleColor: '#f8fafc',
                        bodyColor: '#f8fafc',
                        borderColor: '#334155',
                        borderWidth: 1,
                        padding: 12,
                        cornerRadius: 8,
                        displayColors: true,
                        callbacks: {
                            label: function(context) {
                                return context.label + ': ' + context.parsed + '%';
                            }
                        }
                    }
                },
                animation: {
                    animateRotate: true,
                    animateScale: true,
                    duration: 1500,
                    easing: 'easeOutQuart'
                }
            }
        });
    }

    // ===== PROPERTY TYPES CHART =====
    const propertyTypesChartCanvas = document.getElementById('propertyTypesChart');
    if (propertyTypesChartCanvas) {
        const propertyTypesChart = new Chart(propertyTypesChartCanvas, {
            type: 'doughnut',
            data: {
                labels: ['شقق', 'فيلات', 'أراضي', 'أخرى'],
                datasets: [{
                    data: [45, 30, 15, 10],
                    backgroundColor: [
                        '#06b6d4',
                        '#8b5cf6',
                        '#f59e0b',
                        '#ec4899'
                    ],
                    borderWidth: 0,
                    hoverOffset: 10
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: '#1e293b',
                        titleColor: '#f8fafc',
                        bodyColor: '#f8fafc',
                        borderColor: '#334155',
                        borderWidth: 1,
                        padding: 12,
                        cornerRadius: 8,
                        displayColors: true,
                        callbacks: {
                            label: function(context) {
                                return context.label + ': ' + context.parsed + '%';
                            }
                        }
                    }
                },
                animation: {
                    animateRotate: true,
                    animateScale: true,
                    duration: 1500,
                    easing: 'easeOutQuart'
                }
            }
        });
    }

    // ===== ANIMATE STAT NUMBERS =====
    function animateNumbers() {
        const statNumbers = document.querySelectorAll('.stat-number[data-target]');
        
        statNumbers.forEach(element => {
            const target = parseInt(element.getAttribute('data-target'));
            const duration = 2000;
            const increment = target / (duration / 16);
            let current = 0;

            const timer = setInterval(() => {
                current += increment;
                if (current >= target) {
                    element.textContent = target.toLocaleString('en-US');
                    clearInterval(timer);
                } else {
                    element.textContent = Math.floor(current).toLocaleString('en-US');
                }
            }, 16);
        });
    }

    // Run number animation on load
    setTimeout(animateNumbers, 300);

    // ===== CHART REFRESH BUTTONS =====
    const chartActionBtns = document.querySelectorAll('.chart-action-btn');
    chartActionBtns.forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            const icon = this.querySelector('i');
            
            // Add spin animation
            icon.style.animation = 'spin 1s ease';
            
            setTimeout(() => {
                icon.style.animation = '';
            }, 1000);
            
            // Show toast notification
            showToast('تم تحديث البيانات', 'success');
        });
    });

    // ===== TOAST NOTIFICATION FUNCTION =====
    function showToast(message, type) {
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

        const toast = document.createElement('div');
        toast.className = 'toast-notification';

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

        requestAnimationFrame(() => {
            toast.style.transform = 'translateX(0)';
            toast.style.opacity = '1';
        });

        setTimeout(() => {
            toast.style.transform = 'translateX(-100%)';
            toast.style.opacity = '0';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    // ===== SPIN ANIMATION =====
    const style = document.createElement('style');
    style.textContent = `
        @keyframes spin {
            from { transform: rotate(0deg); }
            to { transform: rotate(360deg); }
        }
    `;
    document.head.appendChild(style);

    console.log('📊 Dashboard Charts Initialized Successfully!');
});
