import os
from flask import Flask, flash, request, redirect, url_for, send_from_directory
import time
import json

app = Flask(__name__)

@app.route('/', methods=['GET', 'POST'])
def upload_motion_history():
    if request.method == 'POST':
        jsonObj = request.json
        filename = time.strftime("%Y%m%d%H%M%S")
        newFile = open('uploads/' + filename + '.json', 'w')
        newFile.write(json.dumps(jsonObj))
    return json.dumps(jsonObj)

if __name__ == '__main__':
    app.run(debug=True, host='127.0.0.1', port=5000)
