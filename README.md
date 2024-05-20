# Репозиторий проекта ObjectDetection

## Технологии Приложения:
* MS .NET 8 Core
* Avalonia

## Конфигурация:
* Скачать файл конфигурации (находится в wiki) и указать полный путь к весам: \src\neuronet\detector\object.detection.onnx
* Положить файл конфигурации в C:\ProgramData\objectDetection.application\Configuration

## Как запустить приложение:
Если нет MS Visual Studio 2022:
1. Убедиться, что установлен пакет SDK для .NET 8.0.
2. Клонировать репозиторий ObjectDetection.
3. Запустить "build.cmd".
4. Запустить exe-файл "objectDetection.gui", расположенный по пути "src/objectDetection.gui.prj/bin/Release/net8.0/objectDetection.gui.exe".

Если установлен MSVS2022, также можно открыть файл решения ObjectDetection.sln и собрать его.

## Примечания
В приложении имеется возможность обработки как видео, так и изображений.

### Сриншоты Приложения:

![alt text](https://github.com/Nanonikich/ObjectDetection/blob/main/screenshots/ObjectDetection.png "Object Detection")

![alt text](https://github.com/Nanonikich/ObjectDetection/blob/main/screenshots/Settings.png "Settings")
