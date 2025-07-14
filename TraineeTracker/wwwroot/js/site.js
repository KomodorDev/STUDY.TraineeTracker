console.log("site.js loaded");


// ------------------------------------------------------
/* script for reloading modal */
function hookUpStateChangeForms() {

    // Browser Print
    console.log("🔄 hookUpStateChangeForms() aufgerufen");
    document.querySelectorAll(".stateChangeForm").forEach(function (form) {
        if (form.querySelector("input[name='TargetStateName']")) {
            form.addEventListener("submit", async function (e) {

                // Browser Print
                console.log("📤 Submit intercepted on form:", form);
                e.preventDefault();

                document.body.classList.add('sopro-waiting-cur');

                try {
                    const formData = new FormData(form);

                    const response = await fetch(form.action, {
                        method: "POST",
                        body: formData
                    });

                    if (response.ok) {
                        const html = await response.text();
                        const modalContent = document.getElementById("TraineeLessonModalContent");

                        // Replace inner modal content only
                        modalContent.innerHTML = html;

                        // Re-hook form events if needed
                        window.hookUpStateChangeForms();
                    } else {
                        alert("Failed to change state.");
                    }
                } catch (error) {
                    alert("An error occured during state change.");
                } finally {
                    document.body.classList.remove('sopro-waiting-cur');
                }
            });
        }
    });
}

window.hookUpStateChangeForms = hookUpStateChangeForms;

// ------------------------------------------------------
window.renderLessonChart = function(config) {
    console.log("Overlay-Werte:", config.effortOverlayMin, config.effortOverlayMax);

    const chartEl = document.getElementById('lessonChart');
    if (!chartEl) return;
  
    const ctx = chartEl.getContext('2d');
  
    const paddedData = [
        ...config.data,
        { x: [0, 0], y: '_' }
    ];

    const paddedColors = [
        ...config.colors,
        'rgba(0,0,0,0)'
    ];

    const effortGapCenterX = (Number(config.effortOverlayMin) + Number(config.effortOverlayMax)) / 2;

    const annotations = {
      today: {
        type: 'line',
        xMin: config.todayPos,
        xMax: config.todayPos,
        borderColor: 'black',
        borderWidth: 2,
        label: {
          content: 'Current Progress',
          enabled: true,
          position: 'start'
        }
      },
      range: {
        type: 'box',
        xMin: config.effortOverlayMin,
        xMax: config.effortOverlayMax,
        yMin: -2,
        yMax: config.data.length + 2,
        backgroundColor: 'rgba(128, 128, 128, 0.15)',
        borderWidth: 0
      }
    };

    if (
      config.PredictedMissingEstimatedEffortAtEnd !== null &&
      config.PredictedMissingEstimatedEffortAtEnd !== 0 &&
      !isNaN(config.PredictedMissingEstimatedEffortAtEnd)
    ) {
      annotations.effortGapLabel = {
        type: 'line',
        xMin: effortGapCenterX,
        xMax: effortGapCenterX,
        borderWidth: 0,
        label: {
          content: 'Predicted Effort Gap',
          enabled: true,
          position: 'start',
          backgroundColor: 'black',
          color: 'white',
          font: {
            weight: 'bold'
          }
        }
      };
    }

    new Chart(ctx, {
      type: 'bar',
      data: {
        datasets: [
          {
            label: 'Effort (days)',
            data: paddedData,
            backgroundColor: paddedColors,
            borderColor: paddedData.map(d => d.y === '__padding__' ? 'rgba(0,0,0,0)' : 'black'),
            borderWidth: 1, 
            borderSkipped: false,
            borderAlign: 'inner',
            categoryPercentage: 1.0,
            barPercentage: 1.0,
            parsing: { xAxisKey: 'x', yAxisKey: 'y' },
            order: 1,
            barThickness: 14,
            maxBarThickness: 16
          }
        ]
      },
      options: {
        indexAxis: 'y',
        responsive: true,
        maintainAspectRatio: false,
        animation: false,
        scales: {
          x: {
            beginAtZero: true,
            title: { display: true, text: 'Estimated Effort in Days' }
          },
          y: {
            type: 'category',
            offset: true,
            grace: '20%',
            ticks: { display: false },
            grid: { drawTicks: false }
          }
        },
        plugins: {
          legend: { display: false },
          annotation: {
            annotations: annotations
          },
          tooltip: {
            enabled: true,
            callbacks: {
              label: function (context) {
                const data = context.raw;
                
                const effort =
                  Array.isArray(data.x) ? (data.x[1] - data.x[0]).toFixed(1) : data.x;

                const status = data.status ?? 'unknown';

                return `Effort (days): ${effort}\nStatus: ${status}`;
              },
              title: function (context) {
                return context[0].label;
              }
            }
          },
        }
      }
    });
  };

  document.querySelectorAll('.info-icon-wrapper').forEach(wrapper => {
    const tooltip = wrapper.querySelector('.tooltip-box');

    wrapper.addEventListener('mouseenter', () => {
        if (!tooltip) return;

        // Reset previous classes
        tooltip.classList.remove('align-left', 'align-right', 'centered');

        const rect = tooltip.getBoundingClientRect();
        const padding = 8; // optionaler Sicherheitsabstand zum Rand

        if (rect.right > window.innerWidth - padding) {
            tooltip.classList.add('align-left');
        } else if (rect.left < padding) {
            tooltip.classList.add('align-right');
        } else {
            tooltip.classList.add('centered');
        }
    });
});
