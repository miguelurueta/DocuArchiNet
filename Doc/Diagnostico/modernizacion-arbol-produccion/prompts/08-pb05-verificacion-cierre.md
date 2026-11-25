# Prompt PB05 para verificar y preparar el cierre de la modernización

**Estado:** borrador de verificación. Requiere los paquetes incluidos implementados y comprobados, PB03 omitido solo si está justificado, y criterios de aceptación D18 aprobados. No autoriza por sí mismo despliegue ni pruebas reales.

## Objetivo

Actúa como arquitecto responsable de aceptación técnica. Lee [el contexto común](contexto-comun.md), la exploración vigente y los resultados de PB00, PF01, PB01, PB02, PB03, PF02 y PB04. Verifica la experiencia integrada y sus contratos. Corrige defectos dentro del alcance aprobado; registra cambios de alcance o de reglas como decisiones que necesitan revisión.

## Auditoría de trazabilidad

1. Relaciona cada decisión aprobada con código y verificación. Distingue lo implementado de lo solo descrito o simulado.
2. Comprueba que los contratos finales coinciden entre backend, JavaScript, página, proyecto y consumidores legacy. Verifica que no cambió una firma por una prueba que solo cubre el nuevo consumidor.
3. Revisa todos los accesos de la matriz PB04. Un menú modernizado no acredita un botón oculto antiguo, una operación de carga o un ASMX que quedó fuera.
4. Identifica cambios compartidos, dependencias nuevas y efectos de dominio. Confirma que no se introdujeron tablas, activaciones por usuario/grupo o cambios de negocio no aprobados.

## Verificación funcional y de autoridad

Cubre al menos los escenarios de la exploración: tipo/estado de cada nodo, nivel vacío, nombres largos/repetidos, identidad compuesta, selección/expansión, búsqueda/filtros/ruta, creación/edición, traslado, ocultos, eliminación, permisos, importación, archivo físico, rótulos/estado/índice y operaciones documentales simples y múltiples.

Exige casos de A/B rápido, clic derecho B con A seleccionado, dos pestañas, limpiar selección, permiso revocado, relación movida/retirada, doble activación, timeout, respuesta inválida y fallo visual después de éxito de dominio. Confirma que el resultado incierto no se convierte en reenvío automático.

Las consultas de contexto/capacidades no deben escribir dominio ni auditoría documental. Valida que las capacidades de presentación no reemplazan la autoridad de ejecución.

## Compilación, navegador y ambiente

- Compila el backend con la configuración real y verifica archivos Compile/Content y empaquetado. Registra las herramientas y límites de compilación.
- Ejecuta las pruebas locales pertinentes y las nuevas pruebas de contrato/comportamiento; no amplíes indefinidamente pruebas sin relación con un cambio o defecto detectado.
- Revisa el DOM WebForms integrado, no solo el de la maqueta: ciclos de UpdatePanel, nodos iniciales/dinámicos, foco y formularios con propietario independiente.
- Valida teclado/táctil, matriz de navegadores, lector de pantalla y zoom acordados en D17. Registra lo probado y lo pendiente.
- Mide volumen/tiempos, consultas y coste de decoración según D16. Establece comparación con la línea base; no uses el pequeño ejemplo como prueba de capacidad.
- Ejecuta pruebas autenticadas o de carga únicamente con ambiente, cuentas y alcance expresamente autorizados conforme al runbook. Si falta esa autorización, entrega el protocolo preparado y marca pendiente la aceptación que depende de él.

## Preparación de publicación y reversión

Describe los archivos/DLL/recursos que deben publicarse juntos, las combinaciones compatibles y el orden de despliegue cuando se haya aprobado. La caché de estáticos no debe mezclar contratos de cliente/servidor incompatibles.

Prepara comprobaciones previas y posteriores y una reversión compatible. No reconstruyas relaciones, borres documentos o reviertas una escritura de negocio para retirar una mejora visual. Si un cambio no admite revertir solo estáticos, declara la dependencia de DLL y no prometas esa reversión parcial.

No publiques ni cambies configuración de ambiente por ejecutar este prompt salvo autorización de despliegue vigente. Si la implementación adoptó un cambio OpenSpec propio, verifica sus artefactos y tareas; no archives automáticamente ni uses DOC-127 como contenedor de esta mejora.

## Entrega de aceptación

Crea `Doc/Diagnostico/modernizacion-arbol-produccion/implementacion/PB05-verificacion-cierre.md` con:

1. Decisiones/versiones y alcance realmente entregado.
2. Matriz de operaciones con pruebas/evidencia y resultado por acceso.
3. Compilación, verificación local, navegador y ambiente claramente diferenciados.
4. Defectos corregidos, pendientes y efecto sobre aceptación/despliegue.
5. Métricas frente a la línea base y límites de lo demostrado.
6. Plan concreto de publicación/reversión y autorizaciones pendientes.
7. Estado de Git y sincronización remota de los cambios propios.

Concluye si la entrega cumple sus criterios aprobados o qué verificaciones faltan. «No encontré fallos en las pruebas ejecutadas» no equivale a garantizar ausencia total de regresiones.
