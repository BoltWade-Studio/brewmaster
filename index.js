
const http = require('http');
const socket = require('socket.io');
const express = require('express');
const path = require('path');
const cors = require('cors');

const app = express();
const server = http.createServer(app);

const port = 11100;


// Serve static files from the WebGL build directory
const buildPath = path.join(__dirname, 'BuildWebGL');
app.use(cors());
app.use(express.static(buildPath));

// Fallback to serve index.html for any route
app.get('*', (req, res) => {
  res.sendFile(path.join(buildPath, 'index.html'));
});


server.listen(port, () => {
  console.log('http://localhost:11100/');
});
