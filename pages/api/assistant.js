export default async function handler(req, res) {
  try {
    if (req.method === 'POST') {
      const { text } = req.body;
      // Απλή απάντηση για δοκιμή
      res.status(200).json({ reply: `Το κείμενο είναι: ${text}` });
    } else {
      res.status(405).json({ error: 'Μέθοδος μη αποδεκτή' });
    }
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: err.message });
  }
}
