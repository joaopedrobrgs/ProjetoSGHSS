# Diagramas – SGHSS

Este diretório contém o diagrama de classes em PlantUML.

Arquivos:
- sghss-model.puml: modelo de domínio (Usuario, Paciente, Profissional, Consulta, Relação)

## Como visualizar

Opção A – VS Code (recomendado)
1) Instale a extensão "PlantUML" (jebbs.plantuml)
2) Abra `sghss-model.puml`
3) Use o comando "PlantUML: Preview Current Diagram"

Opção B – PlantUML Server (sem instalar nada)
- Acesse https://www.plantuml.com/plantuml
- Cole o conteúdo do arquivo `.puml` e gere a imagem.

Opção C – Linha de comando (Java + Graphviz)
```bash
# Requer Java e Graphviz
plantuml -tpng sghss-model.puml  # gera sghss-model.png
```

Inclua a imagem gerada no seu PDF final, na seção de Modelagem/Arquitetura.
