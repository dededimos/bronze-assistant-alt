import { useState } from "react";

export default function TestPage() {
  const [question, setQuestion] = useState("");
  const [response, setResponse] = useState("");

  const askAssistant = async () => {
    setResponse("⏳ Περιμένετε...");
    try {
      const res = await fetch("/api/assistant", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ question }),
      });

      const data = await res.json();
      setResponse("✅ Απάντηση: " + data.answer);
    } catch (error) {
      setResponse("❌ Σφάλμα: " + error.message);
    }
  };

  return (
    <div style={{ fontFamily: "Arial, sans-serif", padding: "40px" }}>
      <h1>🤖 Bronze Assistant – Τεστ Παραγωγής</h1>
      <p>Γράψε την ερώτησή σου και πάτα «Ρώτα»</p>
      <input
        type="text"
        value={question}
        onChange={(e) => setQuestion(e.target.value)}
        placeholder="Π.χ. Ποια καμπίνα είναι κατάλληλη για 120x190;"
        style={{ width: "80%", padding: "10px" }}
      />
      <button onClick={askAssistant} style={{ padding: "10px" }}>
        Ρώτα
      </button>
      <pre style={{ marginTop: "30px", background: "#f0f0f0", padding: "20px" }}>
        {response}
      </pre>
    </div>
  );
}
