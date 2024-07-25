import bpy
import socket
import threading

def handle_connection(client_socket):
    try:
        print("Connexion établie avec le client.")
        while True:
            data = client_socket.recv(1024).decode('utf-8')
            if not data:
                print("Aucune donnée reçue, fermeture de la connexion.")
                break

            print(f"Données reçues : {data}")

            # Parse les données
            try:
                acc_data = data.split(';')[0].split(':')[1].split(',')
                gyro_data = data.split(';')[1].split(':')[1].split(',')

                acc_x, acc_y, acc_z = map(float, acc_data)
                gyro_x, gyro_y, gyro_z = map(float, gyro_data)

                # Appliquer les rotations reçues à la caméra
                bpy.context.scene.camera.rotation_euler = (gyro_x, gyro_y, gyro_z)
                print(f"Rotation appliquée à la caméra : ({gyro_x}, {gyro_y}, {gyro_z})")
            except Exception as e:
                print(f"Erreur de parsing des données : {e}")
    except Exception as e:
        print(f"Erreur de connexion : {e}")
    finally:
        print("Fermeture de la connexion client.")
        client_socket.close()

def start_server():
    try:
        server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server_socket.bind(('0.0.0.0', 12345))
        server_socket.listen(1)
        print("Serveur TCP démarré, en attente de connexion...")

        while True:
            client_socket, address = server_socket.accept()
            print(f"Connexion acceptée de {address}")
            client_thread = threading.Thread(target=handle_connection, args=(client_socket,))
            client_thread.start()
    except Exception as e:
        print(f"Erreur de démarrage du serveur : {e}")

server_thread = threading.Thread(target=start_server)
server_thread.daemon = True
server_thread.start()

print("Le serveur TCP est en cours d'exécution dans un thread séparé.")
