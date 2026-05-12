Ataca primeiro higiene do repositório. Sem isto, tudo o resto fica sujo.

Ordem certa:

1. .gitignore
2. remover bin/ obj/ .DS_Store do Git
3. corrigir docs RTF para Markdown real
4. apagar Class1.cs vazios
5. só depois mexer em Engines

No servidor, faz:

cd C:\SpikeSurfer
notepad .gitignore

Cola isto:

bin/
obj/
.vs/
.DS_Store
*.dll
*.exe
*.pdb
*.cache
*.user
*.suo

Guarda.

Depois:

git rm -r --cached .
git add .
git commit -m "Clean repository and add gitignore"
git push

Depois atacamos os docs:

docs/Architecture.md
docs/MarketStateBible.md
docs/Roadmap.md
docs/SystemStatus.md
docs/SessionLog.md

E só depois:

StructureEngine
RegimeEngine real
SpikeSurferEngine.Read() a usar OpenRange + SecondWave

Resposta ao Claude: primeiro limpar o chão, depois construir paredes.