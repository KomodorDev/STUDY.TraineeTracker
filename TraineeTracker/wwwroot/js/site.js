console.log("site.js loaded");

/* script for reloading modal */
function hookUpStateChangeForms() {
    document.querySelectorAll(".stateChangeForm").forEach(function (form) {
        if (form.querySelector("input[name='TargetStateName']")) {
            form.addEventListener("submit", async function (e) {
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