import threading
import datetime
import os
import io

import sqlite3
import secrets
import hashlib
import base64
import random

from PIL import Image

from Room import Room
from Card import Card

#                                                               for request.form
from flask import Flask, url_for, redirect, g, request, Response, render_template, send_from_directory
from flask_socketio import SocketIO, join_room, leave_room, send, emit, rooms


# Can we create a thread that will clean up expired tokens every 6 hours. Every api call will refresh time of token?
class AuthServer:
    def __init__(self):
        self.sessions = {}

    def new_token(self, uid):
        token = secrets.token_hex()
        self.sessions[token] = {"uid":uid}
        self.extend_token(token)
        return token


    def extend_token(self, token):
        if self.sessions.get(token):
            self.sessions.get(token)["dtlife"] = datetime.datetime.now() + datetime.timedelta(hours=6)


    def is_token_expired(self, token):
        dtnow = datetime.datetime.now()
        isExpired = not (self.sessions.get(token) and dtnow < self.sessions.get(token)["dtlife"])
    
        if not isExpired:
            self.extend_token(token)

        return isExpired


    def del_token(self, token):
        if self.sessions.get(token):
            del self.sessions[token]


    def get_uid(self, token):
        if not auth.is_token_expired(token):
            return str(self.sessions.get(token)["uid"])

auth = AuthServer()


SECRET_KEY = "2738b417b94cce4f7f70953a41f277a3840219d61e32cfc45246949c9dd2c373"
ALLOWED_AVATAR_EXTENSIONS = { "png" }
UPLOAD_AVATAR_FOLDER = "./avatars/"
DATABASE = "./durak.db"
COMISSION = 0.1
DEBUG = True

app = Flask(__name__)
app.config.from_object(__name__)
app.config["SECRET_KEY"] = SECRET_KEY
app.config["UPLOAD_FOLDER"] = UPLOAD_AVATAR_FOLDER
app.config["MAX_CONTENT_LENGTH"] = 3 * 2**20 # 3Mb

socketio = SocketIO(app, logger=DEBUG, engineio_logger=DEBUG)

g_durak_rooms = {}


"""

    DateBase help functions

"""


def get_db():
    # if we are still not db connected
    if not hasattr(g, "link_db"):
        g.link_db = sqlite3.connect(app.config["DATABASE"])

    return g.link_db


@app.teardown_appcontext
def close_db(error):
    # close db connection if it does connected
    if hasattr(g, "link_db"):
        g.link_db.close()


def execSQL(sql, args):
    db = get_db()
    result = None

    try:
        cursor = db.cursor()
        cursor.execute(sql, args)
        result = cursor.fetchall()
        db.commit()

    except Exception as e:
        print("Error db: {0}".format(str(e)))

    if result:
        result = result[0] if len(result) == 1 else result
        result = result[0] if len(result) == 1 else result
    return result

"""

    WS

"""

def cl_distribution(cards, sid):
    emit("cl_distribution", {cards}, to=sid, broadcast=False)

def cl_turn(rid, uid):
    emit("cl_turn", {"uid":uid}, to=rid, broadcast=True)

def cl_grab(rid, uid):
    emit("cl_grab", {"uid":uid}, to=rid, broadcast=True)

def cl_fold(rid, uid):
    emit("cl_fold", {"uid":uid}, to=rid, broadcast=True)

def cl_pass(rid, uid):
    emit("cl_pass", {"uid":uid}, to=rid, broadcast=True)

def cl_start(rid, first, trump, players, bet):
    execSQL('UPDATE Users SET Chips = Chips - ? WHERE ID IN ?;', bet, str(tuple(players)))
    emit("cl_start", {"first":first, "trump":trump}, to=rid, broadcast=True)

def cl_finish(rid, winners):
    for uid in winners.keys():
        execSQL('UPDATE Users SET Chips = Chips + ? WHERE ID == ?;', winners[uid], uid)

    emit("cl_finish", winners, to=rid, broadcast=True)

@socketio.on("srv_whatsup")
def on_srv_whatsup(json):
    uid = auth.get_uid(json["token"])

    if not uid:
        return

    # if user not entered to any room
    if len(rooms()) == 1:
        return

    rid = rooms()[1]
    room = g_durak_rooms[rid]
    
    if not room:
        return

    room.whatsup()

@socketio.on("srv_battle")
def on_srv_battle(json):
    uid = auth.get_uid(json["token"])

    if not uid:
        return

    # if user not entered to any room
    if len(rooms()) == 1:
        return

    rid = rooms()[1]
    room = g_durak_rooms[rid]
    
    if not room:
        return

    attacked = [Card.from_byte(json["attacked"][i]) for i in json["attacked"]]
    attacking = [Card.from_byte(json["attacking"][i]) for i in json["attacking"]]

    if room.battle(uid, attacked, attacking):
        emit("cl_battle", {"uid":uid, "attacked":json["attacked"], "attacking":json["attacking"]}, to=room.get_rid(), broadcast=True)

@socketio.on("srv_transfer")
def on_srv_transfer(json):
    uid = auth.get_uid(json["token"])

    if not uid:
        return

    # if user not entered to any room
    if len(rooms()) == 1:
        return

    rid = rooms()[1]
    room = g_durak_rooms[rid]

    if not room:
        return

    if room.transfer(uid, Card.from_byte(json["card"])):
        emit("cl_transfer", {"uid":uid, "card":json["card"]}, to=room.get_rid(), broadcast=True)
    
@socketio.on("srv_grab")
def on_srv_grab(json):
    uid = auth.get_uid(json["token"])

    if not uid:
        return
    
    # if user not entered to any room
    if len(rooms()) == 1:
        return

    rid = rooms()[1]
    room = g_durak_rooms[rid]

    if room.grab(uid):
        emit("cl_grab", {"uid":uid}, to=room.get_rid(), broadcast=True)

@socketio.on("srv_pass")
def on_srv_pass(json):
    uid = auth.get_uid(json["token"])

    if not uid:
        return

    # if user not entered to any room
    if len(rooms()) == 1:
        return

    rid = rooms()[1]
    room = g_durak_rooms[rid]

    if not room:
        return

    if room.pass_(uid):
        emit("cl_pass", {"uid":uid}, to=room.get_rid(), broadcast=True)

@socketio.on("srv_fold")
def on_srv_fold(json):
    uid = auth.get_uid(json["token"])

    if not uid:
        return

    # if user not entered to any room
    if len(rooms()) == 1:
        return

    rid = rooms()[1]
    room = g_durak_rooms[rid]

    if not room:
        return

    if room.fold(uid):
        emit("cl_fold", {"uid":uid}, to=room.get_rid(), broadcast=True)

@socketio.on("srv_ready")
def on_srv_ready(json):
    uid = auth.get_uid(json["token"])

    if not uid:
        return

    # if player is not in the room
    if len(rooms()) == 1:
        return

    rid = rooms()[1]
    room = g_durak_rooms[rid]

    if not room:
        return

    if float(execSQL('SELECT Chips FROM Users WHERE ID == ?;', (uid,))) < room.get_bet():
        return

    if room.ready(uid):
        emit("cl_ready", {"uid":uid}, to=room.get_rid(), broadcast=True)

@socketio.on("srv_createRoom")
def on_srv_createRoom(json):
    def validRoomJSON(json):
        def validMaxPlayers(mxplayers):
            return 2 <= mxplayers <= 6

        def validGameType(gtype):
            return 0 <= gtype <= 2

        def validNumCards(ncards):
            return ncards == 24 or ncards == 36 or ncards == 52

        def validBet(bet):
            return bet == 100 or bet == 500 or \
                bet == 1000 or bet == 5000 or \
                bet == 10000 or bet == 50000 or \
                bet == 100000 or bet == 200000

        return validMaxPlayers(json["mxplayers"]) and \
            validGameType(json["gtype"]) and \
            validNumCards(json["ncards"]) and \
            validBet(json["bet"]) and \
            json["mxplayers"] * 6 <= json["ncards"]

    uid = auth.get_uid(json["token"])

    if not uid:
        print("not uid")
        return

    if float(execSQL('SELECT Chips FROM Users WHERE ID == ?;', (uid,))) < json["bet"]:
        print("float(execSQL('SELECT Chips FROM Users WHERE ID == ?;', (uid,))) < json[bet]")
        return

    comission = float(execSQL('SELECT Comission FROM Config;'))

    if not validRoomJSON(json):
        print("not validRoomJSON(json)")
        return

    # gen rid
    rid = str(10000 + secrets.randbelow(100000 - 10000))

    # create room
    room = Room(rid, json, comission)

    room.set_distribution_callback(cl_distribution)
    room.set_grab_callback(cl_grab)
    room.set_turn_callback(cl_turn)
    room.set_fold_callback(cl_fold)
    room.set_pass_callback(cl_pass)
    room.set_start_callback(cl_start)
    room.set_finish_callback(cl_finish)
    
    g_durak_rooms[rid] = room

    # add uid
    json["uid"] = uid

    # add rid
    json["rid"] = rid

    # add number of players
    json["players"] = 0

    # remove token
    del json["token"]

    # remove isprivate
    del json["isprivate"]

    # remove key
    del json["key"]

    emit("cl_createRoom", json, broadcast=True)
    print("cl_createRoom")
        

@socketio.on("srv_joinRoom")
def on_srv_joinRoom(json):
    uid = auth.get_uid(json["token"])

    if not uid:
        return

    # get room
    room = g_durak_rooms.get(json["rid"])
        
    # if there is no room by rid
    if not room:
        return

    if float(execSQL('SELECT Chips FROM Users WHERE ID == ?;', (uid,))) < room.get_bet():
        return

    # if room is private and keys is not eq
    if room.is_private() and room.get_key() != json["key"]:
        return

    # get sid
    sid = request.sid

    # add player in room
    # if there is no free place or player is already in room
    if not room.join(uid, sid):
        return

    rid = room.get_rid()

    #join_room(str(room.rid))
    #print(rooms())
    # repeate for RoomServer
    join_room(rid)

    # remove token
    del json["token"]

    # remove key
    del json["key"]

    # add uid
    json["uid"] = uid
                
    json["bet"] = room.get_bet()

    json["ncards"] = room.get_ncards()

    json["gtype"] = room.get_gtype()

    json["players"] = room.getNumberPlayers()

    json["maxplayers"] = room.get_maxplayers()

    # if room is private
    if room.is_private():
        # emit event
        emit("cl_joinRoom", json, to=rid, broadcast=True)
    else:
        # emit event
        emit("cl_joinRoom", json, broadcast=True)

@socketio.on("srv_exitRoom")
def on_srv_exitRoom(json):
    uid = auth.get_uid(json["token"])

    if not uid:
        return

    # get room
    room = g_durak_rooms.get(json["rid"])
        
    # if there is no room by rid
    if not room:
        return

    # remove player from room
    # if player is not in the room
    if not room.leave(uid):
        return

    rid = room.get_rid()

    leave_room(rid)
    # repeate for RoomServer
    #leave_room(str(room.rid))

    # if room is empty
    if room.is_empty():
        # remove room
        del g_durak_rooms[rid]
        return

    # remove token
    del json["token"]

    # add uid
    json["uid"] = uid
        
    json["bet"] = room.get_bet()

    json["ncards"] = room.get_ncards()

    json["gtype"] = room.get_gtype()

    json["players"] = room.getNumberPlayers()

    json["mxplayers"] = room.get_maxplayers()

    # if room is private
    if room.is_private():
        # emit event
        emit("cl_exitRoom", json, to=room.get_rid(), broadcast=True)
    else:
        # emit event
        emit("cl_exitRoom", json, broadcast=True)



# help .html page
@app.route("/helper", methods=["GET"])
def helper():
    return render_template("helper.html")


"""

    WEB API

"""

# /api/login
@app.route("/api/login", methods = ["POST"])
def login():
    name, password = request.form["name"], request.form["password"]
    http_response = ""

    
    #salt = execSQL('SELECT Salt FROM users WHERE Name == ?;', (name))
    
    #if salt:
    #    hash = hashlib.sha3_224()
    #    hash.update(password.encode("utf-8"))
    #    hash.update(salt.encode("utf-8"))
    #    password = hash.hexdigest()

    uid = execSQL('SELECT ID FROM Users WHERE Name == ? and Password == ?;', (name, password))
    if uid and uid >= 0:
        http_response = '{{"token":"{0}", "uid":"{1}"}}'.format(auth.new_token(uid), uid)
    else:
        http_response = '{"error":"The entered data is incorrect"}'

    return Response(response = http_response, content_type="application/json")


# /api/logout?token=kbTiQHNl2D…Rmu
@app.route("/api/logout", methods = ["POST"])
def logout():
    token = request.args.get("token")
    http_response = ""

    if not auth.is_token_expired(token):
        auth.del_token(token)
        http_response = '{"status":"ok"}'
    else:
        http_response = '{"error":"Api token is incorrect"}'

    return Response(response=http_response, mimetype="application/json")


# /api/register_user
@app.route("/api/register_user", methods = ["POST"])
def register_user():
    name, email, password = request.form["name"], request.form["email"], request.form["password"]
    http_response = ""

    salt = secrets.token_hex(16)
    hash = hashlib.sha3_224()
    hash.update(password.encode("utf-8"))
    hash.update(salt.encode("utf-8"))
    password = hash.hexdigest()

    if not execSQL('SELECT * FROM Users WHERE Name=?;', (name,)):
        sql = 'INSERT INTO Users (Name{0}, Password, Salt) VALUES (?{1}, ?, ?);'
        if email:
            sql = sql.format(", Email", ", ?")
            execSQL(sql, (name, email, password, salt))
        else:
            sql = sql.format("", "")
            execSQL(sql, (name, password, salt))

        http_response = '{"status":"ok"}'
    else:
        http_response = '{"error":"This user already exists"}'

    return Response(response=http_response, mimetype="application/json")


# /api/change_email?token=kbTiQHNl2D…Rmu
@app.route("/api/change_email", methods = ["POST"])
def change_email():
    token = request.args.get("token")
    new_email, old_email = request.form["new_email"], request.form["old_email"]
    http_response = ""

    uid = auth.get_uid(token)
    
    if uid:
        if new_email != old_email:
            currentEmail = execSQL('SELECT Email FROM Users WHERE ID=?;', (uid,))
            
            # if user has no email
            if not currentEmail:
                currentEmail = ""

            print(currentEmail)
            if currentEmail == old_email:
                execSQL('UPDATE Users SET Email=? WHERE ID=?;', (new_email, uid))
                http_response = '{"status":"ok"}'
            else:
                http_response = '{"error":"Old email address not found"}'
        else:
            http_response = '{"error":"Email addresses are equal"}'
    else:
        http_response = '{"error":"Api token is incorrect"}'

    return Response(response=http_response, mimetype="application/json")


# /api/get_rating/?token=6t16fy…xFw&limit=50&offset=0
@app.route("/api/get_rating", methods = ["GET"])
def get_rating():
    token = request.args.get("token")
    offset = request.args.get("offset")
    limit = request.args.get("limit")
    http_response = ""

    if not auth.is_token_expired(token):
        expr = 'Won*1.0/"Total"'
        top = execSQL(f'SELECT ID, Name, "Total", {expr} AS WinRate FROM Users WHERE "Total" > 0 ORDER BY WinRate DESC LIMIT ? OFFSET ?;', (limit, offset))
        print(top)
        if top:
            http_response = "["
            for i in top:
                item = '{{"id":{0},"name":"{1}","total":{2},"win_rate":{3}}},'.format(*i)
                http_response += item
            http_response = http_response[:-1] + "]"
        else:
            http_response = '{"error":"Out of range"}'
    else:
        http_response = '{"error":"Api token is incorrect"}'

    return Response(response=http_response, mimetype="application/json")


# /api/get_chips/?token=6t16fy…xFw
@app.route("/api/get_chips", methods = ["GET"])
def get_chips():
    token = request.args.get("token")
    http_response = ""

    uid = auth.get_uid(token)

    if uid:
        chips = execSQL('SELECT Chips FROM Users WHERE ID = ?;', (uid,))
        http_response = '{{"chips":{0}}}'.format(chips)
    else:
        http_response = '{"error":"Api token is incorrect"}'
    
    return Response(response=http_response, mimetype="application/json")


#/api/get_username/13?token=6t16fy…xFw
@app.route("/api/get_username/<int:uid>", methods = ["GET"])
def get_username(uid):
    token = request.args.get("token")
    http_response = ""

    if not auth.is_token_expired(token):
        uname = execSQL('SELECT Name FROM Users WHERE ID = ?;', (uid,))
        http_response = '{{"username":"{0}"}}'.format(uname)
    else:
        http_response = '{"error":"Api token is incorrect"}'

    return Response(response=http_response, mimetype="application/json")


# /api/upload_avatar?token=kbTiQHNl2D…Rmu
@app.route("/api/upload_avatar", methods = ["POST"])
def upload_avatar():
    token = request.args.get("token")
    http_response = ""

    def save_png(avatar, filepath):
        try:
            with Image.open(io.BytesIO(avatar.read())) as img:
                img.thumbnail((128, 128))
                img.save(filepath)
        except:
            return False
        
        return True

    uid = auth.get_uid(token)
    filename = "{0}.png".format(uid)
    avatar = request.files.get("avatar")

    if uid:
        if avatar and save_png(avatar, os.path.join(app.config["UPLOAD_FOLDER"], filename)):
            http_response = '{"status":"ok"}'
        else:
            http_response = '{"error":"Not png"}'
    else:
        http_response = '{"error":"Api token is incorrect"}'

    return Response(response=http_response, mimetype="application/json")


# /api/get_avatar/13?token=6t16fy…xFw
@app.route("/api/get_avatar/<int:uid>", methods = ["GET"])
def get_avatar(uid):
    token = request.args.get("token")
    filename = "{0}.png".format(uid)
    http_response = ""

    if not auth.is_token_expired(token):
        #return redirect(url_for("getAvatar", uid=uid).replace("/api/get_avatar", UPLOAD_AVATAR_FOLDER) + ".png")
        if os.path.exists(os.path.join(app.config["UPLOAD_FOLDER"], filename)):
            return send_from_directory(app.config["UPLOAD_FOLDER"], filename)
    else:
        http_response = '{"error":"Api token is incorrect"}'

    return Response(response=http_response, mimetype="application/json")
    


if __name__ == "__main__":
    #app.run(debug=DEBUG, threaded=True)
    socketio.run(app, debug=DEBUG, use_reloader=False)
