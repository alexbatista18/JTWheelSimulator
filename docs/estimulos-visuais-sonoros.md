# Documentação Técnica — Estímulos Visuais e Sonoros no Simulador de Cadeira de Rodas Motorizada (JTWheelSimulator)

**Data:** 19/08/2026
**Objetivo:** descrever os mecanismos de estimulação visual e sonora já implementados no simulador, para uso em pesquisa/paper de educação física.

---

## 1. Contexto do projeto

O projeto é um simulador de cadeira de rodas motorizada desenvolvido em Unity. O participante navega por uma pista com barreiras laterais, enquanto o sistema mede tempos por segmento, colisões e (em uma das funcionalidades) tempo de reação a estímulos visuais.

Atualmente existem **três mecanismos de estimulação/feedback** implementados no código, dois visuais e um sonoro:

| # | Mecanismo | Tipo | Script responsável |
|---|---|---|---|
| 1 | Mudança de cor da barreira ao colidir | Visual | `BarrierColorChange.cs` |
| 2 | Alarme sonoro ao colidir com a barreira | **Sonoro** | `BarrierColorChange.cs` |
| 3 | Painel vermelho piscante para teste de reação | Visual | `AuditorySignalController.cs` |

⚠️ **Observação importante para o paper:** o script que implementa o estímulo puramente visual de reação está nomeado `AuditorySignalController.cs` — um nome que sugere "sonoro", mas **esse script não reproduz nenhum som**. Vale alinhar isso com o educador físico para não haver confusão sobre qual componente é o responsável pelo áudio (que na verdade é o `BarrierColorChange.cs`).

---

## 2. Estímulo Visual #1 — Feedback de colisão nas barreiras

**Arquivo:** `Assets/Scripts/BarrierColorChange.cs`
**Onde está na pista:** duas barreiras, "Green L" (esquerda) e "Green R" (direita).

- Estado normal (sem contato): cor **verde**, `#4FD157`, ~47% de opacidade.
- Ao colidir (cadeira toca a barreira, via trigger com tag "Player"): a cor muda instantaneamente para **vermelho**, `#FF0900`, ~47% de opacidade.
- Ao sair do contato: a cor volta para verde automaticamente.
- Cada colisão incrementa um contador (separado para esquerda e direita), usado depois para relatório de desempenho por segmento da pista (exportado em CSV via `SegmentTimer.cs` / `TrackTime.cs`, com colunas de tempo do segmento e número de colisões esquerda/direita).

**Função pedagógica/de pesquisa:** feedback visual imediato de erro de trajetória (proximidade/colisão com obstáculo lateral), permitindo quantificar objetivamente o número de colisões por lado e por trecho da pista.

---

## 3. Estímulo Sonoro — Alarme de colisão

**Arquivo:** `Assets/Scripts/BarrierColorChange.cs` (mesma classe do item 2)
**Áudio usado:** `Assets/Audio/alert.mp3` (~52 KB, som 3D/espacializado)

- No mesmo momento em que a barreira fica vermelha (colisão), o sistema toca o som `alert.mp3` uma vez, via `AudioSource.PlayOneShot`.
- Cada barreira tem sua própria fonte de áudio (`Audio Source L` e `Audio Source R`), permitindo que o som seja percebido como vindo do lado esquerdo ou direito (áudio 3D posicional).
- Existe no código um modo opcional de **alarme repetitivo**: enquanto o contato com a barreira persistir, o som pode repetir a cada 0,75 segundo (intervalo configurável). **Esse modo está desativado por padrão** na configuração atual da cena — hoje, o som toca apenas uma vez por evento de colisão, mesmo que o contato continue.

**Função pedagógica/de pesquisa:** reforço sonoro simultâneo ao visual, criando um feedback multissensorial (visual + auditivo) de erro de trajetória. O modo repetitivo (ainda não ativado) poderia ser usado futuramente para simular um "alarme contínuo" em protocolos que exijam maior saliência do estímulo.

---

## 4. Estímulo Visual #2 — Tarefa de reação/atenção (sinal vermelho piscante)

**Arquivo:** `Assets/Scripts/AuditorySignalController.cs` (nome enganoso — é 100% visual, sem som)

Este é um mecanismo independente das barreiras, funcionando como uma **tarefa de tempo de reação** sobreposta à navegação:

- Um painel/retângulo vermelho aparece na tela (Canvas/HUD) em **intervalos aleatórios entre 8 e 12 segundos**.
- O painel permanece visível por **2 segundos** e depois desaparece, reiniciando o sorteio do próximo intervalo.
- O ciclo de estímulos só começa automaticamente quando o cronômetro principal da pista está ativo (sincronizado com `TrackTime.cs`).
- **Medição da resposta:** o sistema monitora a posição da cadeira de rodas frame a frame. Quando o estímulo aparece, ele registra:
  - **Se** o participante parou de se mover em resposta ao estímulo;
  - **Tempo até parar** (uma espécie de tempo de reação, do instante em que o sinal aparece até o instante em que a cadeira para);
  - **Duração da parada** (quanto tempo o participante permaneceu parado antes de voltar a se mover).
- Cada estímulo gera um registro. Ao final da sessão (quando o cronômetro é parado), todos os registros são exportados automaticamente em um arquivo CSV (nomeado por nível de dificuldade), com colunas:
  - Tempo do estímulo (s)
  - Se o jogador parou (sim/não)
  - Tempo até parar (s)
  - Duração parado (s)

**Função pedagógica/de pesquisa:** este é o componente mais diretamente relevante para um paper sobre atenção/tempo de reação — trata-se essencialmente de uma tarefa do tipo *"stop-signal"* (sinal de parada) aplicada ao contexto de condução da cadeira de rodas simulada, com coleta de dados quantitativos prontos para análise estatística (tempo de reação e duração da resposta motora).

---

## 5. Tabela-resumo de parâmetros configuráveis

| Parâmetro | Valor atual | Onde é definido |
|---|---|---|
| Cor da barreira (normal) | `#4FD157`, ~47% opacidade | `BarrierColorChange.cs` |
| Cor da barreira (colisão) | `#FF0900`, ~47% opacidade | `BarrierColorChange.cs` |
| Som de colisão | `alert.mp3` | `BarrierColorChange.cs` / `Assets/Audio/alert.mp3` |
| Alarme sonoro repetitivo | Desativado (intervalo de 0,75s se ativado) | `BarrierColorChange.cs` |
| Intervalo entre estímulos visuais | Aleatório entre 8 e 12 segundos | `AuditorySignalController.cs` |
| Duração do estímulo visual | 2 segundos | `AuditorySignalController.cs` |
| Limiar de detecção de movimento | 0,02 m por frame | `AuditorySignalController.cs` |

---

## 6. Observações finais

- Todos esses parâmetros (intervalos, cores, duração, limiares) são ajustáveis no Unity Inspector, ou seja, podem ser facilmente recalibrados para diferentes protocolos experimentais sem alterar código.
- Vale destacar ao educador físico que **o áudio de alarme e toda a tarefa de reação visual (`AuditorySignalController`) ainda não foram formalmente commitados no controle de versão** — são funcionalidades em desenvolvimento ativo, o que pode ser relevante mencionar caso o paper cite versões específicas do sistema.
- O repositório do projeto não possui, até o momento, documentação metodológica própria além do que foi levantado aqui a partir do código-fonte.
