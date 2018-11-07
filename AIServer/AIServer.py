import os
from flask import Flask, flash, request, redirect, url_for, send_from_directory
import time
import json
import random

app = Flask(__name__)
dirPath = 'uploads'
placeholderHistory = """
Flop1&1
"""

@app.route('/test', methods=['GET', 'POST'])
def test():
    return "hello"

@app.route('/upload', methods=['GET', 'POST'])
def upload_motion_history():
    if request.method == 'POST':
        jsonObj = request.json
        filename = time.strftime("%Y%m%d%H%M%S")
        newFile = open(dirPath + '/' + filename + '.json', 'w')
        newFile.write(json.dumps(jsonObj))
    return json.dumps(jsonObj)

@app.route('/download-random', methods=['GET', 'POST'])
def download_random_history():
    if len(os.listdir(dirPath)) == 0:
        return placeholderHistory
    else:
        random.seed()
        historyFile = open(dirPath + '/' + random.choice([f for f in os.listdir(dirPath) if f.endswith('.json')]), 'r')
        return convertJSONMotionHistory2EasyParsingForm(json.loads(historyFile.read()))

def convertJSONMotionHistory2EasyParsingForm(motionHistoryInJSON):
    out = ''
    motionHistoryInJSON = motionHistoryInJSON['sequence']
    for m in motionHistoryInJSON:
        out += m['motionType']
        out += '&'
        out += str(m['duration'])
        out += '\n'
    if out == '':
        out = 'Still&1.0\n'
    return out

if __name__ == '__main__':
    app.run(debug=False, host='127.0.0.1', port=5000)
