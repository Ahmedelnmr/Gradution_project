/* ===================================================================
   DASHBOARD CHARTS - Chart.js Configuration
   Interactive Donut Charts for Data Visualization - REAL DATA
   ================================================================== */

document.addEventListener('DOMContentLoaded', function () {
    // Chart.js default configuration for RTL and dark theme
    Chart.defaults.font.family = "'Cairo', sans-serif";
    Chart.defaults.color = '#94a3b8';

    // Color palette for consistent theming
    const colors = {
        primary: '#6366f1',
        purple: '#8b5cf6',
        pink: '#ec4899',
        success: '#10b981',
        warning: '#f59e0b',
        cyan: '#06b6d4',
        danger: '#ef4444',
        // Additional colors for property types
        emerald: '#34d399',
        amber: '#fbbf24',
        rose: '#fb7185',
        indigo: '#818cf8',
        violet: '#a78bfa',
        fuchsia: '#e879f9'
    };

    // ===== USERS DISTRIBUTION CHART =====
    const usersChartCanvas = document.getElementById('usersChart');
    if (usersChartCanvas && typeof dashboardData !== 'undefined') {
        const usersChart = new Chart(usersChartCanvas, {
            type: 'doughnut',
            data: {
                labels: ['أصحاب عقارات', 'سماسرة', 'مديري النظام'],
                datasets: [{
                    data: [
                        dashboardData.users.owners,
                        dashboardData.users.agents,
                        dashboardData.users.admins
                    ],
                    backgroundColor: [
                        colors.primary,
                        colors.purple,
                        colors.pink
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
                            label: function (context) {
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

        // Update Legend Values
        updateLegendValue('legend-owners', dashboardData.users.owners);
        updateLegendValue('legend-agents', dashboardData.users.agents);
        updateLegendValue('legend-admins', dashboardData.users.admins);
    }

    // ===== ADS DISTRIBUTION CHART =====
    const adsChartCanvas = document.getElementById('adsChart');
    if (adsChartCanvas && typeof dashboardData !== 'undefined') {
        const adsChart = new Chart(adsChartCanvas, {
            type: 'doughnut',
            data: {
                labels: ['نشطة', 'قيد المراجعة', 'مباعة/مؤجرة', 'مرفوضة'],
                datasets: [{
                    data: [
                        dashboardData.properties.active,
                        dashboardData.properties.pending,
                        dashboardData.properties.soldRented,
                        dashboardData.properties.rejected
                    ],
                    backgroundColor: [
                        colors.success,
                        colors.warning,
                        colors.cyan,
                        colors.danger
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
                            label: function (context) {
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

        // Update Legend Values
        updateLegendValue('legend-active', dashboardData.properties.active);
        updateLegendValue('legend-pending', dashboardData.properties.pending);
        updateLegendValue('legend-sold', dashboardData.properties.soldRented);
        updateLegendValue('legend-rejected', dashboardData.properties.rejected);
    }

    // ===== PROPERTY TYPES CHART =====
    const propertyTypesChartCanvas = document.getElementById('propertyTypesChart');
    if (propertyTypesChartCanvas && typeof dashboardData !== 'undefined') {
        const propertyTypesData = dashboardData.propertyTypes;
        const labels = Object.keys(propertyTypesData);
        const values = Object.values(propertyTypesData);
        const total = values.reduce((a, b) => a + b, 0) || 1;
        const percentages = values.map(v => Math.round((v / total) * 100 * 10) / 10);

        // Generate colors dynamically
        const chartColors = [
            colors.cyan, colors.purple, colors.warning, colors.pink,
            colors.emerald, colors.amber, colors.rose, colors.indigo,
            colors.violet, colors.fuchsia, colors.primary, colors.success
        ];

        const propertyTypesChart = new Chart(propertyTypesChartCanvas, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: percentages,
                    backgroundColor: chartColors.slice(0, labels.length),
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
                            label: function (context) {
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

        // Generate Legend Dynamically
        generatePropertyTypesLegend(labels, percentages, chartColors);
    }

    // ===== HELPER FUNCTIONS =====

    function updateLegendValue(elementId, value) {
        const element = document.getElementById(elementId);
        if (element) {
            element.textContent = value + '%';
        }
    }

    function generatePropertyTypesLegend(labels, percentages, chartColors) {
        const legendContainer = document.getElementById('property-types-legend');
        if (!legendContainer) return;

        legendContainer.innerHTML = '';

        labels.forEach((label, index) => {
            const legendItem = document.createElement('div');
            legendItem.className = 'legend-item';
            legendItem.innerHTML = `
                <span class="legend-dot" style="background: ${chartColors[index]};"></span>
                <span class="legend-label">${label}</span>
                <span class="legend-value">${percentages[index]}%</span>
            `;
            legendContainer.appendChild(legendItem);
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
        btn.addEventListener('click', function (e) {
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

        const toastColors = {
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
            border-right: 4px solid ${toastColors[type] || toastColors.info};
            color: #f8fafc;
            font-family: 'Cairo', sans-serif;
            font-size: 14px;
            transform: translateX(-100%);
            opacity: 0;
            transition: all 0.3s ease;
        `;

        toast.innerHTML = `
            <i class="fas ${icons[type] || icons.info}" style="color: ${toastColors[type] || toastColors.info}; font-size: 18px;"></i>
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

    console.log('📊 Dashboard Charts Initialized Successfully with REAL DATA!');
});
