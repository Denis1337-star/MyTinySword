# MyTinySword

> 2D RTS-прототип на Unity с системой рабочих, добычей ресурсов, Tilemap-картой, NavMeshPlus-навигацией, Cinemachine-камерой и анимациями через Blend Tree.

![Cover](Images/cover.png)

---

# О проекте

**MyTinySword** — это прототип 2D RTS-игры, созданный на Unity как портфолио-проект.

В игре реализован базовый игровой цикл стратегии в реальном времени:

1. игрок управляет юнитами
2. нанимает рабочих
3. отправляет их добывать ресурсы
4. рабочие приносят ресурсы на базу

Основная цель проекта — показать навыки **gameplay programming**, архитектуры игровых систем и организации кода в Unity.

---

# Ключевые механики

### Управление юнитами
- выделение юнитов
- управление через команды
- взаимодействие с окружением

### Экономика
- добыча ресурсов
- перенос ресурсов на базу
- найм новых рабочих

### Игровой мир
- Tilemap карта
- взаимодействие с ресурсами
- постройки

### Анимации
- движение
- idle состояние
- рабочие действия

---

# Использованные технологии

- **Unity**
- **C#**
- **NavMeshPlus**
- **Cinemachine**
- **Tilemap**
- **Rule Tiles**
- **Animator**
- **Blend Tree**
- **Unity UI**

---

# Архитектура проекта

Проект разделён на несколько логических подсистем.

- Input System
- Selection System
- Command System
- Units
- Worker Logic
- Buildings
- Resources
- UI
- Animation

---

# Высокоуровневая архитектура

```mermaid
flowchart TD

Player[Игрок]

Input[Input System]
Selection[Selection System]
Command[Command System]

Units[Units]
Worker[Worker Logic]

Buildings[Buildings]
Resources[Resource Nodes]

Animator[Animator + BlendTree]
NavMesh[NavMeshPlus]

UI[Gameplay UI]

Camera[Cinemachine Camera]

Map[Tilemap + Rule Tiles]

Player --> Input
Input --> Selection
Selection --> Command

Command --> Units
Units --> Worker

Worker --> Resources
Worker --> Buildings

Worker --> Animator
Worker --> NavMesh

UI --> Selection
UI --> Buildings

Map --> NavMesh
Camera --> Player
---

# Взаимодействие скриптов

В проекте используется разделение логики между несколькими основными системами.

Каждая система отвечает за отдельную часть геймплея:

- управление игроком
- обработка ввода
- логика юнитов
- взаимодействие с ресурсами
- работа построек
- UI

Ниже показана схема взаимодействия основных игровых скриптов.

```mermaid
flowchart LR

SelectionSystem[SelectionSystem]
UnitSelectable[UnitSelectable]
CommandSystem[CommandSystem]

Worker[Worker]
House[House]
ResourceNode[ResourceNode]

WorkerAnimator[Animator Controller]
NavMeshAgent[NavMeshPlus Agent]

UIWorkerItem[WorkerListItem]
UI[Gameplay UI]

SelectionSystem --> UnitSelectable
SelectionSystem --> UIWorkerItem

CommandSystem --> Worker

House --> Worker

Worker --> ResourceNode
Worker --> WorkerAnimator
Worker --> NavMeshAgent

UI --> SelectionSystem
UI --> House
UIWorkerItem --> SelectionSystem
