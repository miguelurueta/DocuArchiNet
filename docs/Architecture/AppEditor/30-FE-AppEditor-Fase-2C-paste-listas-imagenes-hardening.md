# Ticket: 30-FE AppEditor Fase 2C paste listas imagenes y hardening

## Identificacion
- Cambio: `30-FE`
- Nombre: `AppEditor Fase 2C - paste, listas, imagenes y hardening`
- Ticket propuesto: `SCRUMCORE-AE-F2C-01`
- Modulo: `src/app/Components/UI/AppEditor/`
- Dependencia previa: `28-FE AppEditor Fase 2B - seleccion, cursor y links`

## Objetivo
Cerrar la fase de produccion con soporte robusto de paste, listas basicas, imagenes y rendimiento razonable.

## Alcance
- paste largo con redistribucion multipagina
- listas basicas entre paginas
- imagenes como bloques indivisibles
- reglas para mover bloques completos cuando no caben
- optimizacion incremental y corte de trabajo redundante
- pruebas de regresion para documentos largos

## No alcance
- tablas complejas
- viudas/huérfanas
- reglas editoriales avanzadas tipo Word

## Criterios de aceptacion
- pegar texto largo no rompe el editor
- listas basicas mantienen continuidad razonable entre paginas
- imagenes que no caben pasan completas a la siguiente pagina
- documentos multipagina normales siguen siendo editables con buen rendimiento
- no hay congelamientos severos en uso normal

## Validacion minima
- paste de texto largo con links
- paste de contenido con listas
- imagen grande que no cabe
- documento de varias paginas con ediciones sucesivas

