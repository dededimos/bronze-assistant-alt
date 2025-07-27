export default function handler(req, res) {
  if (req.method === 'POST') {
    const { text } = req.body;
    // Απλή απάντηση για δοκιμή
    res.status(200).json({ reply: `Το κείμενο είναι: ${text}` });
  } else {
    res.status(405).json({ error: 'Μέθοδος μη αποδεκτή' });
  }
}
