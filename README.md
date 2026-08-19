# JTWheelSimulator — Simulador de Cadeira de Roda Motorizada

Simulador em Unity de uma cadeira de rodas motorizada, desenvolvido para uso em pesquisa/reabilitação em educação física. O usuário navega por uma pista com barreiras laterais, controlando a cadeira por um joystick físico (conectado via porta serial ou Bluetooth), enquanto o sistema mede tempos por segmento, colisões e, em um dos modos, tempo de reação a estímulos visuais.

> Empresa/produto (conforme `ProjectSettings/ProjectSettings.asset`): **InnovaDTA** — **Simulador CRM** (`com.InnovaDTA.SimuladorCRM`).

---

## Sumário

- [Requisitos](#requisitos)
- [Instalação do projeto no Unity](#instalação-do-projeto-no-unity)
- [Estrutura de cenas](#estrutura-de-cenas)
- [Arquitetura / Scripts principais](#arquitetura--scripts-principais)
- [Conexão da cadeira / joystick físico](#conexão-da-cadeira--joystick-físico)
- [Gerando o APK (build Android)](#gerando-o-apk-build-android)
- [Regras de negócio](#regras-de-negócio)
- [Pontos de atenção e boas práticas](#pontos-de-atenção-e-boas-práticas)

---

## Requisitos

- **Unity Editor `2022.3.22f1` (LTS)** — usar exatamente essa versão (instalar via **Unity Hub**) para evitar reimportação/quebra de cenas e assets. A versão exata fica registrada em `ProjectSettings/ProjectVersion.txt`.
- **Módulos do Unity Hub necessários:**
  - Android Build Support (com **SDK & NDK Tools** e **OpenJDK**, marcados na instalação do módulo) — obrigatório para gerar o APK.
- **Visual Studio** (ou VS Code) com o workload de desenvolvimento de jogos com Unity. O arquivo `.vsconfig` na raiz do repositório já descreve o workload mínimo necessário (`Microsoft.VisualStudio.Workload.ManagedGame`) — ao abrir o `.sln` do projeto no Visual Studio Installer, ele pode sugerir instalar os componentes faltantes automaticamente.
- **Git** para clonar/versionar o repositório.
- Para testar com o hardware físico da cadeira: um joystick/módulo de controle pareado por **Bluetooth** (celular Android) ou conectado por **cabo/adaptador serial** ao PC (modo Editor).

---

## Instalação do projeto no Unity

1. Clone o repositório:
   ```bash
   git clone <url-do-repositorio>
   ```
2. Abra o **Unity Hub** → **Add project from disk** → selecione a pasta clonada.
3. Se o Unity Hub não tiver a versão `2022.3.22f1` instalada, ele vai oferecer para instalar automaticamente ao tentar abrir o projeto. Aceite a instalação dessa versão específica.
4. Abra o projeto. Na primeira abertura o Unity vai reimportar todos os assets — isso pode levar alguns minutos.
5. Na janela **Project**, abra a cena principal em `Assets/Scenes/Fase1.unity` (é a única cena atualmente habilitada em **File > Build Settings**).
6. Para testar sem o hardware físico, use o modo de teclado (ver script `MovementeTeclado.cs`, movimento com WASD) — não é necessário nenhum periférico para rodar o simulador no Editor.

---

## Estrutura de cenas

| Cena | Situação | Descrição |
|---|---|---|
| `Assets/Scenes/Fase1.unity` | ✅ Habilitada em Build Settings | Cena principal/ativa do simulador — cronômetro, barreiras, colisões, estímulo visual e conexão da cadeira. Historicamente já foi renomeada de `Inicio.unity`. |
| `Assets/Scenes/CenaMenu.unity` | ⚠️ Não incluída no Build Settings atual | Menu inicial do app. Precisa ser adicionada manualmente em **File > Build Settings** para compor o fluxo completo (Menu → Fase1). |
| `Assets/Scenes/Fase2.unity` | ⚠️ Não incluída no Build Settings atual | Segunda pista/fase. Também precisa ser adicionada manualmente se for usada no build. |
| `Assets/Scenes/SampleScene.unity` | Não utilizada | Cena padrão gerada pelo Unity na criação do projeto; não faz parte do fluxo do app. |
| `Assets/Scenes/Pista/` | Assets auxiliares | Prefabs de pista/obstáculos (`Coppell Stadium_Model_fbx`, `Obstaculos`, `Publicidade`, `NetworkManager`, luz e câmera). |
| `Assets/Scenes/Inicio/` | Assets auxiliares | Splines (`SplineExtrude_*`) usados para desenhar o trilho da pista. |

> **Atenção:** antes de gerar qualquer build, confira **File > Build Settings** e adicione as cenas necessárias (`CenaMenu`, `Fase1`, `Fase2` conforme o fluxo desejado) — hoje só `Fase1` está habilitada.

---

## Arquitetura / Scripts principais

Todos em `Assets/Scripts/`.

| Script | Responsabilidade |
|---|---|
| `TrackTime.cs` | Cronômetro principal da corrida. Exporta CSV de tempos por segmento + colisões (`Application.persistentDataPath`). Reposiciona a cadeira conforme a dificuldade selecionada (`Easy` / `Medium` / `Hard`) e controla a tela de "chegada" em modo VR. |
| `SegmentTimer.cs` | Cronometra cada trecho da pista via *trigger colliders* (tag `Player`), soma o tempo ao `TrackTime` e zera os contadores de colisão a cada segmento. |
| `BarrierColorChange.cs` | Barreiras laterais mudam de cor (verde → vermelho) ao colidir e tocam um alarme sonoro (`Assets/Audio/alert.mp3`). Mantém contadores estáticos globais de colisão à esquerda/direita. |
| `AuditorySignalController.cs` | Tarefa de estímulo/resposta visual: exibe um sinal na tela em intervalos aleatórios (8–12s) e mede o tempo de reação do usuário (quando ele para e por quanto tempo), exportando CSV próprio (`*_estimulos.csv`). |
| `WheelchairControllerUSB.cs` | Controla a cadeira via **porta serial (COM)** — usado no modo Editor/PC. |
| `WheelchairControllerBluetooth.cs` | Controla a cadeira via dados recebidos por **Bluetooth nativo Android**, com suporte a inversão de eixo. |
| `BluetoothManager.cs` | Gerencia a conexão Bluetooth Classic no Android (plugin nativo `.aar`), permissões em runtime e timeout de conexão. |
| `MovementeTeclado.cs` | Movimento alternativo via teclado (WASD), útil para testar sem hardware físico. |
| `ButtonMove.cs` | Movimento via Input System (gamepad/joystick genérico). |
| `CameraFollow.cs` | Câmera em terceira pessoa, segue e rotaciona ao redor da cadeira. |
| `ColissionCounter.cs` | Soma o total de colisões a partir do texto exibido em tela. |
| `ColissionDetector.cs` | Wrapper genérico de `OnTriggerEnter` que dispara um `UnityEvent`. |
| `ConfigButton.cs` / `CloseConfigButton.cs` | Abrem/fecham o painel de configurações. |
| `ConfirmationController.cs` | Aplica a dificuldade selecionada (escala do circuito, posição inicial da cadeira) ao confirmar no menu. |
| `MenuManagar.cs` | Alterna entre o painel de menu e o componente alvo. |
| `ObjectController.cs` / `SeeCube.cs` | Sistema de "olhar para confirmar" (gaze) do menu em modo VR (Google Cardboard). |

Documentação detalhada dos estímulos visuais e sonoros: [`docs/estimulos-visuais-sonoros.md`](docs/estimulos-visuais-sonoros.md).

---

## Conexão da cadeira / joystick físico

O projeto suporta **dois modos de conexão distintos**, dependendo de onde o simulador está rodando. **Não existe conexão por IP/rede** — a comunicação é feita por **Bluetooth Classic (RFCOMM)** ou por **porta serial (COM)**, não por TCP/IP.

### 1. Modo Editor / PC — Porta Serial (COM)

Usado pelo script `WheelchairControllerUSB.cs` (o nome menciona "USB", mas na prática ele lê uma porta serial — seja um adaptador USB-serial real, seja uma porta COM virtual criada por um módulo Bluetooth SPP pareado no Windows).

Parâmetros configuráveis no Inspector do componente:

| Parâmetro | Valor padrão | Descrição |
|---|---|---|
| `portName` | `COM6` | Porta serial do dispositivo. **Deve ser ajustada** para a porta em que o joystick/módulo aparece no seu PC (verifique em *Gerenciador de Dispositivos > Portas (COM & LPT)*). |
| `baudRate` | `9600` | Velocidade de transmissão — deve ser igual à configurada no firmware do dispositivo. |

O dispositivo deve enviar dados no formato de texto `"x,y"` (dois inteiros separados por vírgula), lidos a cada frame e processados por `ProcessBluetoothData(string data)`.

### 2. Modo Android (APK) — Bluetooth Classic nativo

Usado pelos scripts `WheelchairControllerBluetooth.cs` + `BluetoothManager.cs`, via plugin Android nativo `Assets/Plugins/Android/unity3dbluetoothplugin-release.aar`.

- **Endereço MAC do módulo Bluetooth da cadeira está hardcoded** em `BluetoothManager.cs`:
  ```csharp
  private readonly string deviceMAC = "10:52:1C:5D:F8:26";
  ```
  **Se o módulo Bluetooth físico da cadeira for trocado, esse MAC precisa ser atualizado no código** (não há tela de configuração/pareamento dinâmico no app atualmente).
- Permissões solicitadas em runtime pelo app: `CoarseLocation`, `FineLocation`, `BLUETOOTH_ADMIN`, `BLUETOOTH`, `BLUETOOTH_SCAN`, `BLUETOOTH_ADVERTISE`, `BLUETOOTH_CONNECT`.
  > ⚠️ Essas permissões **não estão declaradas** em `Assets/Plugins/Android/AndroidManifest.xml`. Confirme que o manifest final do build as inclui (via merge do plugin ou manualmente), senão a solicitação de permissão pode falhar silenciosamente no APK gerado.
- Timeout de tentativa de conexão: **5 segundos**.
- Mesmo formato de dados do modo serial: string `"x,y"`, com baseline `baselineX = 1550` e limites `baselineX ± 300/200` para decidir frente/trás/esquerda/direita. Suporta inversão de eixo via `SetInvertMovement`.

---

## Gerando o APK (build Android)

Configurações atuais (`ProjectSettings/ProjectSettings.asset`):

| Configuração | Valor |
|---|---|
| Application ID (Android) | `com.InnovaDTA.SimuladorCRM` |
| Version Code | `3` |
| Min SDK | `27` (Android 8.1 Oreo) |
| Target SDK | `33` (Android 13) |
| Scripting Backend | **IL2CPP** |
| Orientação de tela | Auto Rotation |

Passos para gerar o APK:

1. **File > Build Settings** → confirme a plataforma **Android** selecionada (*Switch Platform* se necessário).
2. Confirme as cenas incluídas na build (ver seção [Estrutura de cenas](#estrutura-de-cenas)).
3. Em **Player Settings**, confirme o **Publishing Settings > Keystore**. O projeto usa um keystore local (`user.keystore`, ver [Pontos de atenção](#pontos-de-atenção-e-boas-práticas)) para assinar o APK.
4. Clique em **Build** e escolha o destino do arquivo `.apk`.
5. Instale no celular Android via `adb install caminho\para\o.apk` ou transferindo o arquivo diretamente.

### Suporte a VR (Google Cardboard)

O projeto inclui o **Google XR Cardboard Plugin** (`com.google.xr.cardboard`, ver `Packages/manifest.json`) e os assets `Assets/XR/`. O menu em modo VR usa um sistema de "olhar para confirmar" (`ObjectController.cs`/`SeeCube.cs`, originados do exemplo oficial do Cardboard). Isso permite rodar o simulador em modo estereoscópico (celular + óculos Cardboard) além do modo de tela normal — não é necessário nenhum passo extra de build além dos acima, mas vale testar ambos os modos ao validar um novo build.

---

## Regras de negócio

- **Dificuldade:** o usuário escolhe entre `Easy`, `Medium` e `Hard` no menu inicial (`ConfirmationController.cs`). Cada dificuldade define a escala do circuito e a posição inicial da cadeira (`TrackTime.cs`).
- **Cronometragem:** o tempo é medido por segmento da pista (via triggers, `SegmentTimer.cs`) e no total da corrida (`TrackTime.cs`). Ao final, os dados são exportados em CSV local no dispositivo, com nome do segmento, tempo e colisões à esquerda/direita.
- **Colisões:** cada contato com uma barreira lateral conta como uma colisão (separada por lado), muda a cor da barreira para vermelho e toca um alarme sonoro (`alert.mp3`). Os contadores são reiniciados a cada segmento.
- **Estímulo de reação:** de forma independente das colisões, um sinal visual aparece em intervalos aleatórios de 8–12 segundos, permanecendo visível por 2 segundos. O sistema mede se e quando o usuário reage parando a cadeira, exportando os dados de cada tentativa em CSV separado. Ver detalhes completos em [`docs/estimulos-visuais-sonoros.md`](docs/estimulos-visuais-sonoros.md).
- **Modo de entrada:** o app aceita entrada tanto do controle físico da cadeira (serial ou Bluetooth, conforme a plataforma) quanto do teclado (para testes), sem exigir hardware para rodar no Editor.

---

## Pontos de atenção e boas práticas

Itens identificados no repositório atual que merecem cuidado de quem for trabalhar no projeto:

- **`user.keystore` está versionado no Git.** É a chave de assinatura do APK Android — idealmente não deveria estar em um repositório versionado (ainda mais se o repositório for público). Considere migrar para uma variável de ambiente/keystore local por máquina e remover do histórico se necessário, avaliando com a equipe o impacto de reescrever o histórico.
- **APKs compilados (`jstwheell.apk`, `Fase1.apk`) estão versionados no Git**, incluindo arquivos de dezenas de MB. Recomenda-se **não commitar binários de build** — usar Releases do GitHub ou outro storage para distribuir o APK, e adicionar `*.apk` ao `.gitignore`.
- **`Logs/` e `UserSettings/` não estão no `.gitignore`** e aparecem versionados (logs de importação do Editor, layouts de janela). Esses arquivos são específicos de cada máquina/instalação e geram diffs desnecessários a cada commit — recomenda-se adicioná-los ao `.gitignore`.
- **Os `.csproj`/`.sln` são regenerados automaticamente pelo Unity** a cada abertura do editor e também aparecem frequentemente modificados nos commits — normalmente não precisam ser versionados.
- **O MAC address do módulo Bluetooth está hardcoded** em `BluetoothManager.cs` (ver seção de conexão) — ao trocar o hardware da cadeira, é necessário atualizar o código-fonte e gerar um novo build.
- **O nome do script `AuditorySignalController.cs` é enganoso**: apesar do nome sugerir estímulo sonoro, ele implementa apenas o estímulo **visual** de reação — o alarme sonoro real fica em `BarrierColorChange.cs`. Ver nota detalhada em [`docs/estimulos-visuais-sonoros.md`](docs/estimulos-visuais-sonoros.md).
- Sugestão de entradas a adicionar ao `.gitignore`: `Logs/`, `UserSettings/`, `*.apk`, `*.keystore`, `[Oo]bj/`.

---

## Contato

Dúvidas sobre o projeto:

- **Telefone:** (69) 99369-7356
- **E-mail:** dkalexbatista@gmail.com
