# Prompt: 31-FE AppEditor Fase 2C paste listas imagenes y hardening

Actua como arquitecto y desarrollador senior especializado en TipTap, ProseMirror, paste multipagina, listas, imagenes y performance.

Necesito implementar `AppEditor Fase 2C - paste, listas, imagenes y hardening` sobre las fases previas ya completadas.

## Objetivo
- hacer robusto el paste multipagina
- soportar continuidad razonable de listas basicas
- tratar imagenes como bloques indivisibles
- cerrar la fase con rendimiento estable para uso normal

## Restricciones
- no congelar la UI con repaginacion completa innecesaria
- no dejar contenido corrupto tras paste largo
- no cortar imagenes visualmente

## Casos obligatorios
- paste largo con formato inline
- listas basicas repartidas entre paginas
- imagen que pasa completa a la siguiente hoja
- documento multipagina con varias operaciones seguidas

## Entregables
- implementacion real del hardening final
- optimizacion incremental razonable
- pruebas de paste, listas, imagenes y documentos largos

## Criterios de aceptacion
- el editor sigue estable en uso multipagina normal
- paste, listas e imagenes no rompen el flujo
- el rendimiento es defendible para produccion

