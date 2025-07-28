async function getCabinInfo(userText) {
  try {
    const response = await fetch('/api/cabin', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ text: userText })
    });

    console.log("📡 Status:", response.status);

    if (!response.ok) {
      let data = {};
      try {
        data = await response.json();
      } catch (_) {
        console.warn("⚠️ Δεν μπορέσαμε να διαβάσουμε JSON από το API.");
      }
      console.error("❌ API error full response:", response);
      throw new Error(`API error: ${data.error || response.statusText}`);
    }

    const result = await response.json();
    console.log("📦 Απάντηση από το API:", result);
    return result;

  } catch (error) {
    console.error("❌ Σφάλμα κατά την κλήση API:", error);
    throw error;
  }
}

async function submitCabinRequest() {
  const userText = document.getElementById('userText').value.trim();

  if (!userText) {
    document.getElementById('result').innerHTML = '⚠️ Παρακαλώ γράψτε περιγραφή.';
    return;
  }

  document.getElementById('result').innerHTML = '⏳ Επεξεργασία...';

  try {
    const cabinInfo = await getCabinInfo(userText);

    // fallback debug mode αν δεν υπάρχει details
    if (!cabinInfo || typeof cabinInfo !== 'object') {
      document.getElementById('result').innerHTML = '❌ Άκυρη απάντηση από το σύστημα.';
      return;
    }

    if (!cabinInfo.details) {
      document.getElementById('result').innerHTML = `
        <p><strong>🔗 Link καμπίνας:</strong> 
          <a href="${cabinInfo.cabin_url}" target="_blank">Προβολή στο BronzeApp</a>
        </p>
        <p><em>(⚠️ Δεν επιστράφηκαν τεχνικές λεπτομέρειες)</em></p>
        <p><strong>Debug:</strong> ${cabinInfo.internal_debug_code || '(δεν υπάρχει)'}</p>
      `;
      return;
    }

    const details = cabinInfo.details;

    document.getElementById('result').innerHTML = `
      <p><strong>✅ Κωδικός καμπίνας (debug):</strong> ${cabinInfo.internal_debug_code}</p>
      <p><strong>🔗 Link καμπίνας:</strong> 
        <a href="${cabinInfo.cabin_url}" target="_blank">Προβολή στο BronzeApp</a>
      </p>
      <hr>
      <h4>📋 Λεπτομέρειες:</h4>
      <p><strong>Μοντέλο:</strong> ${details.model_key?.toUpperCase() || '—'}</p>
      <p><strong>Διαστάσεις:</strong> ${details.width || '?'} x ${details.height || '?'} cm</p>
      <p><strong>Πάχος γυαλιού:</strong> ${details.glass_key || '—'}</p>
      <p><strong>Extras:</strong> ${details.extras?.length ? details.extras.join(', ') : '—'}</p>
    `;
  } catch (error) {
    document.getElementById('result').innerHTML = `<span style="color:red;">❌ Σφάλμα: ${error.message}</span>`;
  }
}
