const socket = io.connect('http://localhost:3000')
const sender = document.getElementById('sender')
const message = document.getElementById('message')
const submitBtn = document.getElementById('submitBtn')
const messages = document.getElementById('messages')
const typing = document.getElementById('typing')
submitBtn.addEventListener('click', ()=> {
    socket.emit('chat', {
        message: message.value,
        sender: sender.value
    })
})
socket.on('chat', data => {
    typing.innerHTML = '';
    messages.innerHTML += '<p><strong>' + data.sender + ': </strong>' + data.message + '</p>'
    message.value = '';
})
message.addEventListener('keypress', () => {
    socket.emit('typing', sender.value)
})
socket.on('typing', data => {
    typing.innerHTML = '<p>' + data + ' yazıyor...</P>'
})