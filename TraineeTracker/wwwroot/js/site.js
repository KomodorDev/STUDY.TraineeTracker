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
    const chartEl = document.getElementById('lessonChart');
    if (!chartEl) return;
  
    const ctx = chartEl.getContext('2d');
  
    new Chart(ctx, {
      type: 'bar',
      data: {
        datasets: [
          {
            label: 'Predicted Effort Range',
            data: config.data.map(d => ({ x: [config.effortOverlayMin, config.effortOverlayMax], y: d.y })),
            backgroundColor: 'rgba(255, 165, 0, 0.25)',
            parsing: { xAxisKey: 'x', yAxisKey: 'y' },
            order: 0,
            barThickness: 14,
            maxBarThickness: 16
          },
          {
            label: 'Effort (days)',
            data: config.data,
            backgroundColor: config.colors,
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
            title: { display: true, text: 'Effort' }
          },
          y: {
            type: 'category',
            ticks: { display: false },
            grid: { drawTicks: false }
          }
        },
        plugins: {
          legend: { display: false },
          tooltip: { enabled: true },
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