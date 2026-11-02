<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12 -->
## Purpose

Integrar la importación documental por carpetas desde el nivel seleccionado de Producción, con preparación, recepción, coordinación y recuperación durables, preservando los consumidores existentes.

## ADDED Requirements

### Requirement: RQ-01 — Apertura y destino fijo
El sistema SHALL abrir la importación desde un nivel existente elegible, resolver su ruta y autoridad en servidor y fijar destino/ámbito en plan y lote, sin otro árbol ni redirección por navegación.

Trazabilidad: D-01, RQ-01; diseño e inventario comunes.

#### Scenario: Cambio de contexto
- **WHEN** otra pestaña cambia la selección o el nivel fijado cambia de ubicación/permiso
- **THEN** el lote conserva su destino y la operación afectada se rechaza para revisión, sin mover expedientes

#### Scenario: Creación bajo el nivel
- **WHEN** se importan dos raíces elegibles
- **THEN** cada expediente queda relacionado con el nivel confirmado y su relación se verifica; gabinete y nivel no se confunden

### Requirement: RQ-02 — Facultad operativa y transporte
El sistema SHALL separar IMPORTAR_CARPETAS de administrar perfiles, revalidar identidad activa/facultades/recursos por acción y proteger JSON y multipart contra solicitudes falsificadas. SHALL NOT elevar administración para reutilizar una lectura.

Trazabilidad: D-02, RQ-02; diseño e inventario comunes.

#### Scenario: Importador sin administración
- **WHEN** un usuario autorizado para importar carece de administración
- **THEN** puede obtener perfil publicado compatible y preparar su operación sin acceder a mutaciones administrativas; solo administrar no concede importar

#### Scenario: Contexto inválido
- **WHEN** se pierde sesión, se revoca permiso o llega un token ajeno/ausente
- **THEN** se rechaza la siguiente solicitud protegida antes de efectos y sin datos del recurso; esquema/permiso ausente no concede acceso

### Requirement: RQ-03 — Inventario completo y rutas
El sistema SHALL conservar identidades por selección, raíz y archivo, validar manifiesto completo y cuotas servidor y tratar nombres/rutas como datos no confiables.

Trazabilidad: D-03, RQ-03; diseño e inventario comunes.

#### Scenario: Homónimos y cambios
- **WHEN** se agregan raíces o archivos homónimos y luego se cambia inventario o perfil
- **THEN** se conservan identidades distintas y se invalida la preparación anterior; fecha/tamaño/nombre no prueban igualdad de contenido

#### Scenario: Rutas y capacidad
- **WHEN** hay traversal, ruta absoluta, colisión normalizada, exceso o configuración de límites inválida
- **THEN** se rechaza sin construir una ruta física cliente; carpetas vacías no se fabrican y las subcarpetas siguen la política del perfil

#### Scenario: Perfil publicado sin habilitación adicional en Web.config
- **WHEN** existe un perfil publicado compatible y el usuario tiene sesión, destino y permisos válidos, sin claves `Doc111.*` en appSettings
- **THEN** la capacidad reutiliza formatos/tamaño del perfil y límites técnicos del transporte/JSON existentes, sin exigir configuración adicional ni aumentar límites de IIS
- **AND** el plan conserva una vigencia derivada de la sesión existente; la ausencia de una política explícita de limpieza conserva los originales y no bloquea importar

### Requirement: RQ-04 — Plan y valores autorizados
El sistema SHALL preparar desde una versión publicada compatible, resolver reglas/catálogos/proveedores en servidor, conservar snapshot/huellas y devolver incidencias/elegibilidad sin crear expedientes ni documentos.

Trazabilidad: D-04, RQ-04; diseño e inventario comunes.

#### Scenario: Campos diferidos
- **WHEN** una fuente depende de un expediente todavía inexistente
- **THEN** se conserva dependencia tipada para materializar después; el preview no crea ni fabrica ID y puedeConfirmar procede del servidor

#### Scenario: Plan ajeno o cambiado
- **WHEN** se reenvía plan manipulado/ajeno, perfil cambió o manifiesto está incompleto
- **THEN** se rechaza confirmación sin sustituir por otra versión; errores globales se distinguen de incidencias por elemento

#### Scenario: Materialización posterior a creación
- **WHEN** un archivo recibido conserva dependencias del expediente ya confirmado
- **THEN** se resuelven solo esas dependencias con el resolvedor/catálogos operativos y se conservan los valores previos; una incompatibilidad bloquea el documento sin eliminar ni recrear el expediente

#### Scenario: Valores efectivos ya fijados
- **WHEN** se repite una fase con valores efectivos persistidos, incluida una colección vacía
- **THEN** se devuelve el mismo conjunto y procedencia sin recalcular fuentes ni consultar proveedores; se mantiene la autorización vigente del contexto y los recursos

### Requirement: RQ-05 — Confirmación y persistencia
El sistema SHALL confirmar un plan de forma transaccional e idempotente, fijar usuario/ámbito/nivel/configuración/inventario y registrar estados/intentos/eventos propios en MySQL 5.1. La aplicación SHALL NOT ejecutar migraciones automáticamente.

Trazabilidad: D-05, RQ-05; diseño e inventario comunes.

#### Scenario: Confirmación concurrente
- **WHEN** dos clientes confirman el mismo plan o repiten la misma clave y contenido
- **THEN** se obtiene el mismo lote sin duplicar raíces/archivos; clave igual y contenido distinto produce conflicto

#### Scenario: Esquema operativo
- **WHEN** falta o es incompatible el esquema de operación
- **THEN** se rechaza la capacidad nueva sin aplicar DDL ni romper Producción/carga individual; la entrega incluye SQL manual completo, controles y recuperación parcial

### Requirement: RQ-06 — Recepción segura
El sistema SHALL recibir un archivo esperado por operación autorizada mediante streaming a staging privado, verificar bytes reales/formato/hash y separar recepción de resultado documental.

Trazabilidad: D-06, RQ-06; diseño e inventario comunes.

#### Scenario: Contenido y repetición
- **WHEN** se reenvía el mismo archivo, un contenido distinto o bytes fuera del límite
- **THEN** la repetición compatible no duplica recepción; contenido distinto/exceso/formato inválido se rechazan sin afirmar incorporación

#### Scenario: Fallo parcial de recepción
- **WHEN** se pierde respuesta o existe discordancia entre parte privada y estado persistido
- **THEN** se consulta/reconcilia antes de repetir, se conserva evidencia y nunca se expone una ruta física ni se borra staging por cerrar ventana

### Requirement: RQ-07 — Efectos documentales comprobados
El sistema SHALL usar adaptadores internos caracterizados para crear y almacenar/incorporar sin modificar SII o contratos compartidos; SHALL verificar IDs, relación al nivel, gabinete, índices e incorporación.

Trazabilidad: D-07, RQ-07; diseño e inventario comunes.

#### Scenario: Nombre original y campos efectivos del gabinete
- **WHEN** se verifica un documento almacenado mediante el flujo existente
- **THEN** inventario e índice electrónico deben conservar exactamente el nombre original y la columna NOMBRE del gabinete también debe coincidir cuando sea visible
- **AND** si NOMBRE no está configurado o no es visible, se admite vacío o NULL además del nombre exacto; otro nombre, configuración ambigua o visibilidad inválida no permiten confirmar, sin modificar la configuración compartida ni repetir almacenamiento

#### Scenario: Nombre y temporales
- **WHEN** el almacenamiento consume una copia de trabajo y toma su nombre físico
- **THEN** el original recuperable se conserva y el nombre/extensión documental se verifica; no se duplica incorporación o índice ya efectuado

#### Scenario: Reutilización del almacenamiento con alcance ajustado
- **WHEN** se implementa el adaptador documental conforme a la decisión explícita del usuario del 2026-10-30
- **THEN** se reutiliza la función de almacenamiento intacta sin exigir resolver el aislamiento adicional del XML posterior al commit frente a otras cargas; se conservan autoridad, exclusión propia de DOC-111, verificación de relaciones e incertidumbre sin replay automático, sin afirmar exclusión global demostrada

#### Scenario: Efecto sin correlación
- **WHEN** hay crash después de crear/almacenar y no puede demostrarse su correspondencia con la operación
- **THEN** se conserva INCIERTO y se bloquea replay automático; un ID aislado o HTTP 200 no acreditan éxito ni habilitan un adaptador no caracterizado

#### Scenario: Ciclo de archivo de Producción
- **WHEN** se prepara la creación de una raíz de DOC-111
- **THEN** se usa «Archivo Gestión», elegido explícitamente por el usuario, se resuelve su ID en RA sin fijarlo por defecto y se rechazan ausencia, ambigüedad o nombre discrepante; el ciclo resuelto debe conservarse en la evidencia de la futura creación

### Requirement: RQ-08 — Barreras y ejecución acotada
El sistema SHALL conservar modo/ámbito publicados, ejecutar una fase acotada por petición y aplicar la barrera completa antes de iniciar efectos de la unidad exigida, sin prometer transacción global.

Trazabilidad: D-08, RQ-08; diseño e inventario comunes.

#### Scenario: Completo por expediente
- **WHEN** la política es COMPLETO_OBLIGATORIO/EXPEDIENTE y solo una raíz recibió/validó todos sus archivos
- **THEN** solo esa raíz puede avanzar; la otra permanece visible bloqueada

#### Scenario: Completo por lote
- **WHEN** la política es COMPLETO_OBLIGATORIO/LOTE y falta un archivo válido
- **THEN** no inicia efectos de ninguna raíz hasta cumplir la barrera global

#### Scenario: Parcial y fallo posterior
- **WHEN** PARCIAL_PERMITIDO tiene elementos elegibles o falla una materialización tras crear
- **THEN** parcial procesa elegibles con ámbito NO_APLICA; efectos previos y todos los totales originales se conservan y una unidad incompleta no se marca completa

### Requirement: RQ-09 — Exclusión, recuperación y detención
El sistema SHALL reservar intentos persistidos, revalidar autoridad antes de efectos y recuperar resultados con evidencia; timeout, cierre o lease vencida SHALL NOT autorizar repetición ciega.

Trazabilidad: D-09, RQ-09; diseño e inventario comunes.

#### Scenario: Cierre explícito de reserva de almacenamiento nunca iniciada
- **WHEN** un operador autorizado pide cerrar una reserva ALMACENAR con revisión vigente y se comprueba transaccionalmente que sigue RESERVADO, sin evidencia, fin ni diario de inicio o resultado
- **THEN** se conserva el expediente confirmado y los valores/archivo recibido, se registra el rechazo del intento por cierre previo al inicio y se libera solo su reclamación con incremento de revisión y evento durable, sin ejecutar almacenamiento
- **AND** el cierre y el inicio compiten por el mismo bloqueo; si el inicio ocurrió se rechaza el cierre, y si el cierre ocurrió se rechaza el comando anterior; repetir el cierre nunca libera otra reclamación ni convierte un resultado incierto en pendiente

#### Scenario: Dos ejecutores
- **WHEN** dos peticiones intentan reclamar el mismo recurso o vence una reclamación con efecto aún posible
- **THEN** solo una inicia la fase; la expiración no habilita otro efecto sin demostrar terminación/no ejecución

#### Scenario: Detener y reconciliar
- **WHEN** se solicita detención, se revoca permiso o se pierde respuesta después de un efecto
- **THEN** no se inician nuevos efectos afectados; se conservan resultados, se consulta y solo se reconcilia con autoridad/evidencia, sin marcarExitoso cliente ni rollback destructivo

### Requirement: RQ-10 — Seguimiento y mantenimiento
El sistema SHALL ofrecer consultas paginadas y acciones autorizadas desde estado durable, exportar resultados seguros y aplicar retención/limpieza solo mediante política explícita.

Trazabilidad: D-10, RQ-10; diseño e inventario comunes.

#### Scenario: Consultar y exportar
- **WHEN** un usuario autorizado consulta o descarga resultados
- **THEN** no se ejecutan efectos/reconciliaciones; el CSV protege fórmulas y no incluye secretos o paths

#### Scenario: Retención y limpieza
- **WHEN** no hay política válida o existe incertidumbre que necesita evidencia
- **THEN** solo se permite diagnóstico/listado en seco; no se borran staging, intentos o historial necesarios para recuperar

### Requirement: RQ-11 — Interfaz y reanudación
El sistema SHALL integrar una ventana propia con destino fijo y etapas del recorrido aprobado, progreso comprobable y recursos/eventos acotados tras UpdatePanel, sin simulación productiva.

Trazabilidad: D-11, RQ-11; diseño e inventario comunes.

#### Scenario: Cerrar y reabrir
- **WHEN** se cierra/recarga la ventana o se abre otra preparación desde otro nivel
- **THEN** el lote confirmado conserva destino/ID; se consulta su estado y se pide reselección de archivos no recibidos sin prometer acceso automático al disco

#### Scenario: Mostrar y actualizar
- **WHEN** se presentan nombres externos y resultados documentales confirmados
- **THEN** se usa texto seguro, páginas acotadas y progreso diferenciado; el árbol usa IDs/relaciones reales sin duplicar nodos ni cambiar selección actual

### Requirement: RQ-12 — Verificación progresiva y compatibilidad
La entrega SHALL reutilizar herramientas existentes, probar capacidades por fase y REC-01/02 integrados, conservar carga individual/SII y distinguir evidencia local, real y no ejecutada.

Trazabilidad: D-12, RQ-12; diseño e inventario comunes.

#### Scenario: REC y regresión
- **WHEN** se ejecuta aceptación autorizada del conjunto en completo/EXPEDIENTE, completo/LOTE y parcial
- **THEN** dos raíces producen expedientes/documentos bajo destino correcto, índices/totales/relaciones comprobados y repetición/reanudación conserva IDs sin duplicación; controles de datos son SELECT

#### Scenario: Sin autorización o dependencia
- **WHEN** no existe autorización específica de ambiente/cuenta/datos, esquema aplicado o adaptador seguro
- **THEN** las pruebas reales quedan NO_EJECUTADO con causa y las partes independientes usan dobles declarados; no hay nueva plataforma E2E, worker opcional ni PASS por simulación
