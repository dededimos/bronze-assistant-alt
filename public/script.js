async function getCabinInfo(userText) {
  try {
    const response = await fetch('/api/cabin', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ text: userText })
    });

    if (!response.ok) {
      let data = {};
      try { data = await response.json(); } catch (_) {}
      throw new Error(`API error: ${data.error || response.statusText}`);
    }

    return await response.json();
  } catch (error) {
    console.error("❌ Σφάλμα κατά την κλήση API:", error);
    throw error;
  }
}

async function submitCabinRequest() {
  const userText = document.getElementById('userText').value.trim();

  if (!userText) {
    document.getElementById('result').innerHTML = '⚠️ Παρακαλώ πληκτρολογήστε περιγραφή καμπίνας.';
    return;
  }

  try {
    const cabinInfo = await getCabinInfo(userText);
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
      <p><strong>Extras:</strong> ${details.extras.length ? details.extras.join(', ') : '—'}</p>
    `;
  } catch (error) {
    document.getElementById('result').innerHTML = `❌ Σφάλμα: ${error.message}`;
  }
}
