## 1.1 Allgemeine Projektinformationen

Projektname: The Shooting Range

Kurze Beschreibung:
The Shooting Range soll ein Tutorial Level simulieren. Man benutzt eine Waffe, um Kugeln abzuschiessen, die durch den Raum fliegen.

Steuerung:
- Waffe greifen: Die Waffe liegt mitten im Raum. Man greift sie mit dem rechten Controller, indem man die hintere linke Taste drückt
- Schießen: Mit der vorderen hinteren Taste des rechten Controllers schießt man Kugeln ab

Ablauf:
Man schießt auf das rote Board, um das Level zu beginnen. Nach dem Start fliegen Kugeln durch den Raum. Sie prallen an den Wänden und anderen Objekten (zb auch an der Waffe) ab. Ziel ist es, die fliegenden Kugeln abzuschiessen.
Um das Level zu beenden, schießt man erneut auf das rote Board.


## 1.2 Externe Ressourcen

3D-Modelle und Texturen:
- Sci-Fi Handgun:
  - Autor: Dimitri.Rey.3D
  - Quelle: https://sketchfab.com/3d-models/sci-fi-handgun-e36a1c4cf8584fad9d24aab63c641ccd
  - Lizenz: CC-BY-4.0
  - Veränderungen: Waffe auf 0,1 skaliert und um 90 Grad gedreht; Hitbox hinzugefügt
  
Plugins und Bibliotheken:
- Meta XR SDK 207.0.1: Core SDK (OVRCameraRig, Controller-Eingabe), Interaction SDK und MR Utility Kit (liest den echten Raum und die Waende ein)
- Unity OpenXR Plugin 1.18.0 und XR Management 4.7.0
- Unity Input System 1.20.0
- Universal Render Pipeline 17.3.0
- glTFast 6.10.1 (Import des glTF-Modells)
- TextMeshPro (Texte auf dem Board)

Custom Sachen:
- Das rote Board (WallBoard.prefab) besteht aus UI-Image und TextMeshPro-Texten. Die Farbe wechselt im Code (standartmßig auf rot, wenn man es abschießt grün, bzw. rot wenns davor grün war)
- Die Kugeln sind Unity-Primitive-Kugeln mit dem eigenen Material UnlitBase. Das Abprallen kommt von einem im Code erzeugten PhysicsMaterial
- Wände abhängig vom gescanntem Raum


## 1.3 Technische Details und Installation

Unity-Version: 6000.3.26f1 (Unity 6.3)

Dateistruktur (alles Eigene liegt in Assets/):
- Scenes/ enthält Szene abgabe_1.unity
- Scripts/ enthält Skripte:
    HandgunController: Greifen und Schießen mit der Waffe
    GunBullet: Verhalten der Spielerkugeln
    SphereSwarmSpawner: Spawnen und Steuern der fliegenden Kugeln
    FlyingSphere: Verhalten einer einzelnen fliegenden Kugel
    BoardTrigger: Level starten und beenden per Schuss aufs Board
    WallBoardSpawner: Platzieren des Boards an der Hauptwand des Raums
- Prefabs/ enthält das rote Board
- Materials/ enthält das Material der Kugeln
- Models/sci-fi-handgun/ enthält das Waffenmodell
- Oculus/, Resources/, XR/, Plugins/Android/, Settings/ und TextMesh Pro/ enthält Konfiguration von Meta SDK, XR und Render Pipeline

Installationsanleitung:
1. Das Projekt in Unity öffnen
2. Die Szene in Assets/Scenes/abgabe_1.unity öffnen
3. Build and run


## 1.4. Eigenleistung und Reflexion

Den größten Zeitaufwand hatte bei mir das Setup. Danach habe ich die Grundlagen des Projekts selbst gebaut. Die Szene laden, das Waffenmodell importieren und eine Hitbox mit Interaktion hinzufügen. Außerdem habe ich dafür gesorgt, dass das Board an der Wand des gescannten Raums platziert wird und den Passthrough eingerichtet. Auch einfache Features habe ich zuerst selbst probiert, zum Beispiel eine einzelne Kugel, die zufällig durch den Raum fliegt.

Die KI hat mir dann geholfen, das bereits vorhandene deutlich besser umzusetzen und neue Features einzubauen. Dazu gehören das Abprallen der Kugeln an den Wänden und das Abschießen der fliegenden Kugeln mit der Waffe. Auch bei den Formulierungen und Fachbegriffen in 1.2 und 1.3 hat mir die KI geholfen.

Reflexion:
Die größten Probleme hatte ich beim Setup der VR-Brille. Zu Hause habe ich lange versucht, sie einzurichten, aber sie wurde beim Build nicht gefunden. Grund war der Developer Modus, der deaktiviert war. Somit konnte ich für eine lange Zeit meine vorhanden Szene nicht testen, da selbst bis jetzt das Setup vom Meta-XR-Simulator nicht funktioniert. Auch bei den ersten Schritten kam es oft zu Problemen. Ich hatte zuerst nicht verstanden, warum nach dem Build nicht gleich die neue Szene zu sehen war. Teilweise wurde nur die alte Szene angezeigt, manchmal kam die Meldung, dass das Projekt nicht initialisiert wurde.

Da ich vorher keine Erfahrung mit C-Sharp hatte, war die KI bei der Programmierung eine große Hilfe. Damit konnte ich alles umsetzen, was ich mir vorgenommen hatte.

