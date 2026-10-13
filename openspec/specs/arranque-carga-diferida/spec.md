# arranque-carga-diferida Specification

## Purpose

Reducir el trabajo y las solicitudes de la primera apertura del visor PDF sin perder capacidades opcionales ni compatibilidad con su integración WebForms.

## Requirements

### Requirement: Arranque mínimo del visor
El visor SHALL renderizar el primer canvas sin descargar ni ejecutar dependencias de Wacom, impresión, Fabric, Prettify o configuración de grafo. <!-- RQ-01; D-01 -->

#### Scenario: Apertura de solo lectura
- **WHEN** el usuario abre un documento y no activa herramientas opcionales
- **THEN** el primer canvas aparece usando únicamente el núcleo declarado y no existen solicitudes de capacidades opcionales

#### Scenario: Navegación básica
- **WHEN** el documento básico ya está visible
- **THEN** scroll, zoom, rotación y navegación continúan operativos sin cargar capacidades opcionales

### Requirement: Worker PDF aislado del frame
El visor SHALL ejecutar el worker de PDF.js mediante `GlobalWorkerOptions.workerSrc` y SHALL NOT cargar worker, sandbox o worker-entry como scripts normales del frame. <!-- RQ-02; D-02 -->

#### Scenario: Primer render con worker
- **WHEN** `pdfjsLib.getDocument` procesa el documento
- **THEN** PDF.js resuelve su worker dedicado con la misma versión del núcleo y completa el primer canvas

#### Scenario: Artefactos no ejecutables en el frame
- **WHEN** se inspeccionan scripts y errores de la página
- **THEN** `pdf.worker.entry.min.js`, `pdf.worker.min.js` y `pdf.sandbox.min.js` no aparecen como scripts del documento y no generan el error atribuido a DOC-101

### Requirement: Propietario único de dependencias
El visor SHALL mantener un único propietario compatible por familia de jQuery UI, Popper, Font Awesome, Prettify y Wacom, y SHALL NOT cargar recursos sin consumidor demostrado. <!-- RQ-03; D-03 -->

#### Scenario: Contrato estático de duplicados
- **WHEN** se analiza `index.html`
- **THEN** cada familia permitida satisface la cardinalidad y versión definidas en el inventario aprobado

#### Scenario: Compatibilidad del propietario conservado
- **WHEN** se usan dropdown, modal, iconos, arrastre, redimensión o menú contextual
- **THEN** el símbolo proviene del propietario seleccionado y el comportamiento visible se conserva

### Requirement: Carga opcional memoizada y ordenada
El visor SHALL cargar cada capacidad opcional con una promesa compartida, orden determinista y verificación de su símbolo final. <!-- RQ-04; D-04 -->

#### Scenario: Activación concurrente
- **WHEN** una capacidad se solicita varias veces mientras está cargando
- **THEN** todas las solicitudes reciben la misma promesa y cada recurso se inserta como máximo una vez

#### Scenario: Capacidad ya disponible
- **WHEN** una capacidad completada vuelve a solicitarse
- **THEN** el cargador reutiliza su estado sin crear nuevos nodos ni solicitudes

### Requirement: Activación en el punto de uso
El visor SHALL solicitar Wacom, impresión, Fabric, Prettify y configuración de grafo únicamente desde la acción que consume cada capacidad. <!-- RQ-05; D-05 -->

#### Scenario: Wacom
- **WHEN** el usuario activa la tableta digital
- **THEN** la cadena Wacom se carga una vez en orden y después se invoca `captureFromSTU`

#### Scenario: Impresión
- **WHEN** el usuario solicita imprimir
- **THEN** los recursos de impresión se cargan antes de invocar `printJS`

#### Scenario: Configuración de grafo o firma
- **WHEN** el usuario activa una acción que requiere configuración de sello o plugins de grafo
- **THEN** esos recursos y el WebMethod correspondiente se solicitan en ese momento y no durante la apertura básica

### Requirement: Fallo opcional localizado
El visor SHALL mantener el PDF operativo cuando una dependencia opcional no pueda cargarse y SHALL informar el fallo únicamente en la capacidad afectada. <!-- RQ-06; D-06 -->

#### Scenario: Recurso opcional ausente
- **WHEN** falla un script, estilo o símbolo final de una capacidad opcional
- **THEN** la acción muestra un error comprensible, habilita reintento y conserva lectura, scroll y zoom

#### Scenario: Reintento
- **WHEN** el usuario repite una capacidad cuya carga anterior falló
- **THEN** el cargador realiza un nuevo intento sin duplicar recursos que sí terminaron correctamente

### Requirement: Compatibilidad y política de recursos
DOC-101 SHALL conservar contratos WebForms, versiones mayores, orígenes remotos y metadatos de despliegue fuera de los cambios expresamente aprobados. <!-- RQ-07; D-07 -->

#### Scenario: Revisión de entrega
- **WHEN** se inspecciona el diff
- **THEN** no existen cambios de transporte, persistencia, permisos, payloads, bundler o framework y no se agregaron CDN nuevos

#### Scenario: Recurso local nuevo
- **WHEN** la implementación incorpora un archivo estático local
- **THEN** el recurso se registra en `GestionDocumental-Docuarchi.net.vbproj` y conserva la política de despliegue existente

### Requirement: Evidencia reproducible de arranque
DOC-101 SHALL medir solicitudes, `domInteractive`, primer canvas y errores mediante la infraestructura E2E saneada existente. <!-- RQ-08; D-08 -->

#### Scenario: Validación autorizada
- **WHEN** existe autorización vigente y se ejecutan tres iteraciones con el documento aprobado
- **THEN** la evidencia compara la primera apertura con 1.679 ms, las recargas con 92–98 ms y registra hitos, solicitudes y errores sin datos sensibles

#### Scenario: Validación no autorizada
- **WHEN** no existe autorización vigente para ambiente, cuenta y documento
- **THEN** no se ejecuta E2E real y se registra el bloqueo sin inventar resultados
