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
// Opening rejection reason & feedback form
function toggleRejectionForm() {
  const form = document.getElementById("rejectionForm");
  form.style.display = (form.style.display === "none" || form.style.display === "") ? "block" : "none";
}

function toggleFeedbackForm() {
  const form = document.getElementById("feedbackForm");
  form.style.display = (form.style.display === "none" || form.style.display === "") ? "block" : "none";
}


// ------------------------------------------------------
window.renderLessonChart = function(config) {
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

    new Chart(ctx, {
      type: 'bar',
      data: {
        datasets: [
          {
            label: 'Predicted Effort Range',
            data: paddedData.map(d => ({
              ...d,
              x: [config.effortOverlayMin, config.effortOverlayMax]
            })),
            backgroundColor: 'rgba(255, 165, 0, 0.25)',
            parsing: { xAxisKey: 'x', yAxisKey: 'y' },
            order: 0,
            barThickness: 14,
            maxBarThickness: 16
          },
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
          annotation: {
            annotations: {
              today: {
                type: 'line',
                xMin: config.todayPos,
                xMax: config.todayPos,
                borderColor: 'black',
                borderWidth: 2,
                label: {
                  content: 'current progress',
                  enabled: true,
                  position: 'start'
                }
              }
            }
          }
        }
      }
    });
  };