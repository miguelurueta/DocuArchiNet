# Prompt 08 — Creación y resolución de expedientes

## Encargo

Lee guía 00, prompt 03, diseño 04 y contratos operativos 07. Implementa el adaptador de expedientes, sin cambiar SII ni crear escrituras documentales que omitan reglas existentes.

## Evidencia a verificar

Inspecciona `Gestion/ClassGaExpediente.vb`, `Registrar_Expediente_Conservacion`, y la composición en `Gestion/ClassGaProducionDocumental.vb`, `Agregar_expediente_produccion_carpeta_a_la_estructura`. El segundo recorrido depende de controles de interfaz y no es por sí mismo un servicio reusable. Verifica parámetros, tipos de unidad, fechas, responsables, clasificación, nivel padre y efectos físicos/XML. Respeta los límites funcionales observados, incluidos los del código aunque su columna permita más caracteres.

## Trabajo

- Recibir un comando con identidad de operación/raíz, autoridad validada, datos efectivos, nivel padre, ámbito y revisión. No obtener destino de selección mutable de sesión ni cambiar controles para simular navegación.
- Exigir que la coordinación haya habilitado la unidad conforme a la política de integridad. No convertir una llamada al adaptador en mecanismo para saltarse la espera de archivos.
- Consultar resultado previo de la misma raíz: si está confirmado, verificar pertenencia, relación al nivel y estado, y devolverlo. Diferenciar reanudación del mismo lote de coincidencia con otro.
- Definir y aplicar la política de código aprobada. No usar nombre visible como clave única global, ni concatenar sufijos para resolver conflictos silenciosamente.
- Revalidar permisos y nivel antes de crear; cambios de ámbito/ubicación requieren revisión. No crear estructuras nuevas ni asignar un autorregistro para obtener valores predeterminados.
- Invocar la operación existente compatible mediante un adaptador nuevo. Verificar ID de expediente y relación real con el nivel confirmado; conservar ambos identificadores mediante el repositorio operativo.
- Examinar fallos entre creación legacy y registro de resultado. Correlacionar solo con evidencia fiable: un nombre coincidente no demuestra que la operación creó ese expediente. Si no se puede determinar el resultado, registrar INCIERTO y bloquear recreación automática.
- No borrar expedientes con efectos parciales para simular rollback. Reportar fase y recuperación permitida sin exponer SQL o rutas sensibles al usuario.

## Aceptación y entrega

Después de crear y verificar el expediente, devolver los datos reales requeridos por fuentes diferidas de índices. No convertirlos en reglas nuevas ni modificar la versión del perfil. Caracterizar también concurrencia con la creación manual existente: una exclusión propia del lote no protege otros escritores. No exponer la creación como endpoint que permita saltarse la habilitación de 10; el contrato interno y sus precondiciones se prueban aisladamente antes de integrar el coordinador.

Prueba dos raíces en un nivel, nombre repetido en ámbitos distintos, reejecución de la misma raíz, permisos revocados, nivel movido, código inválido y fallo después de crear pero antes de registrar. Demostrar que no se usa un destino nuevo por cambiar la selección del árbol. Entregar matriz de dependencias de sesión/efectos, adaptador, contrato de resultado conocido/rechazado/incierto y pruebas. Reutilizar repositorios 07; cualquier DDL adicional se entrega como SQL MySQL 5.1 coordinado, nunca contra SII.
