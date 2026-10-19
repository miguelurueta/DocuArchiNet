# Prompt 06 — Validación y plan de importación

## Encargo

Lee guía 00, prompts 02/03 y diseño 04. Implementa el servicio de preparación/preflight sin crear expedientes ni documentos. Depende del perfil publicado, sus reglas y los contratos definidos; no construir un segundo motor de reglas.

## Contrato y trabajo

Esta entrega es propietaria del servicio de validación del contexto de apertura y de preparación del plan. La persistencia de plan, si se requiere, se consume de 07-A conforme al diseño 04; 07-B depende del preflight terminado, no al revés. No crear repositorio ni script paralelo para resolver esa dependencia.

Recibe contexto del nivel de apertura, referencia de perfil/versión e inventario completo con identidades estables. La identidad autenticada y ámbito efectivo se resuelven en servidor. Devuelve plan identificado/versionado, resultados por raíz/archivo, valores resueltos, incidencias y posibilidad de confirmar.

1. Valida forma, cuotas, duplicación de identificadores, rutas relativas y completitud del manifiesto. Si es fragmentado, no habilites revisión final hasta recibir todos sus fragmentos comprobados.
2. Recupera versión publicada y configuración autorizada. Valida nivel, gabinete, permisos y coherencia de clasificación/tipologías. Obtener un perfil no concede permisos de creación/carga.
3. Aplica agrupación explícita: una raíz por expediente en el nivel recibido; subcarpetas conforme al perfil. No fabricar carpetas vacías ni niveles.
4. Usa el resolvedor para constantes, valores de carpeta, nombre de archivo y datos del expediente. Distingue código de tipología de ID interno. Una asignación final por campo; conserva ceros iniciales cuando corresponda y valida límites funcionales además de físicos.
5. Resuelve política de código de expediente y conflictos por identidad/ámbito, no solo nombre. Las consultas previas no garantizan ausencia de carreras; dejar explícita la comprobación al ejecutar.
6. Aplica COMPLETO_OBLIGATORIO o PARCIAL_PERMITIDO y su ámbito confirmado. Clasifica errores impeditivos/advertencias y unidades afectadas. En completo no excluyas inválidos del denominador ni autorices efectos parciales. En parcial reporta cada rechazado.
7. Conserva o protege el plan mediante el mecanismo definido en 04, asociado a usuario/ámbito, versión, inventario y destino. No aceptar valores finales reenviados por el cliente como autoridad. Define expiración/revisión y qué cambios lo invalidan.
8. La ausencia de contenido binario limita la validación a metadatos; no marcar como verificados formato real, huella o recepción. Documenta valores que solo se validarán al recibir.

## Aceptación

Implementar ambos ámbitos aprobados. En COMPLETO_OBLIGATORIO/EXPEDIENTE, una raíz elegible puede confirmarse/procesarse sin omitir del inventario otras raíces bloqueadas; cada raíz espera todos sus archivos recibidos y validados. En COMPLETO_OBLIGATORIO/LOTE, un error impeditivo en cualquier archivo bloquea la confirmación global y la ejecución espera recepción/validación total. Errores globales bloquean siempre. El ámbito proviene de la versión del perfil, no de un cambio libre en la ventana. No confundir elegibilidad del plan con recepción ya completada.

Probar errores globales frente a errores individuales: en parcial debe ser posible confirmar unidades elegibles manteniendo rechazados visibles. Devolver `puedeConfirmar`, alcance bloqueado y motivos. Para IDs/consecutivos inexistentes y campos heredados que dependen de ellos, emitir fuentes diferidas tipadas; nunca IDs ficticios ni un valor definitivo inventado. La materialización posterior debe validar la misma regla/versionado y conservar valores efectivos sin alterar el plan aprobado.

Pruebas de dos formatos de nombre configurables, tipo desconocido, campo requerido, conversión/longitud, subcarpetas, raíz homónima, permiso revocado, nivel ajeno, inventario incompleto, plan alterado y cambio de perfil. Comprobar completo/parcial según ámbito confirmado, sin efecto documental ni llamada al adaptador de creación. El mismo inventario/configuración produce un plan determinista salvo campos de control documentados.

Entrega servicio, entrada HTTP compatible, repositorio de lectura/plan según diseño y pruebas. Si guardar planes necesita DDL, coordina su propiedad con el script operativo del 07 y entrega SQL manual compatible; no crear un repositorio alternativo de lotes.
