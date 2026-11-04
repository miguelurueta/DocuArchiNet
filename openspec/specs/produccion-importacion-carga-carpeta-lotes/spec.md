# produccion-importacion-carga-carpeta-lotes Specification

## Purpose

Definir la confirmación idempotente de planes y la recepción recuperable de archivos en lotes de importación, conservando autoridad, integridad, trazabilidad y separación de los efectos documentales definitivos.

## Requirements

### Requirement: RQ-01 Inventario y esquema únicos
El sistema SHALL cumplir El mapa diferencia archivos existentes de propuestas y el SQL operativo referencia el perfil sin recrearlo. Decisión: D-01.

#### Scenario: Mapa de reutilización
- **WHEN** se revisan los componentes 07
- **THEN** el inventario identifica propietario, existencia y consumidor sin crear repositorios paralelos

#### Scenario: Instalación ya existente
- **WHEN** el esquema operativo ya existe
- **THEN** se usan pre/postcomprobaciones SELECT y no se reaplica el script base

### Requirement: RQ-02 Confirmación idempotente
El sistema SHALL cumplir Dos confirmadores del mismo plan conservan un lote; la misma clave con otro payload devuelve conflicto. Decisión: D-02.

#### Scenario: Confirmación simultánea
- **WHEN** dos conexiones confirman el mismo plan con payload idéntico
- **THEN** existe un lote con snapshot e inventario originales y ambos resultados convergen a su identidad

#### Scenario: Clave con payload distinto
- **WHEN** dos planes del mismo ámbito/solicitante usan la misma clave
- **THEN** solo uno puede crear lote y el otro devuelve conflicto sin insertar inventario adicional

### Requirement: RQ-03 Autorización de recepción
El sistema SHALL cumplir Archivo/lote cruzado, campos de destino o estado y petición sin autoridad se rechazan antes de escribir staging. Decisión: D-03.

#### Scenario: ID cruzado
- **WHEN** se envía un archivo perteneciente a otro lote aunque el nombre coincida
- **THEN** la autoridad rechaza el recurso y no escribe bytes

#### Scenario: Multipart no autorizado
- **WHEN** se omite token o se inyecta gabinete, expediente o estado
- **THEN** se rechaza la petición sin mutación operacional

### Requirement: RQ-04 Integridad física y límites
El sistema SHALL cumplir Solo bytes completos y PDF válido pasan a RECIBIDO; rutas de cliente nunca forman la ruta física y el reenvío no duplica cuota. Decisión: D-04.

#### Scenario: Contenido truncado o formato inválido
- **WHEN** el stream termina antes de bytesPrevistos o no es PDF válido
- **THEN** no aparece un completo válido ni RECIBIDO

#### Scenario: Ruta y cuota
- **WHEN** se suministra ruta maliciosa o el contenido excede límites efectivos
- **THEN** la escritura queda contenida o rechazada sin superar cuota; .part se mantiene como evidencia no recibida

### Requirement: RQ-05 Recepción recuperable
El sistema SHALL cumplir Corte de red, completo sin commit, fila sin temporal y respuesta perdida conservan estados verificables sin sobrescribir contenido. Decisión: D-05.

#### Scenario: Respuesta perdida
- **WHEN** el commit de recepción termina y el cliente pierde la respuesta
- **THEN** consulta y reenvío devuelven la recepción previa sin nueva contabilidad

#### Scenario: Disco y fila divergentes
- **WHEN** queda un completo sin commit o una fila con contenido ausente
- **THEN** solo evidencia íntegra permite reconciliar; ausencia bloquea reescritura automática

#### Scenario: Contenido cambiado
- **WHEN** se reenvía otra huella bajo la identidad confirmada
- **THEN** se rechaza CONTENIDO_DISTINTO y se conserva el primer contenido

### Requirement: RQ-06 Concurrencia y conteos
El sistema SHALL cumplir Dos receptores no generan dos commits ni doble contabilidad; vencimiento o token nuevo no autoriza otra escritura incierta. Decisión: D-06.

#### Scenario: Dos recepciones
- **WHEN** dos ejecutores reclaman el mismo archivo durante una escritura
- **THEN** solo una reclamación escribe y se confirma una vez; el otro obtiene resultado o conflicto seguro comprobado

#### Scenario: Cuota concurrente
- **WHEN** dos archivos compiten por un límite agregado finito inyectado
- **THEN** partes y completos contabilizados no exceden el límite y replay no suma bytes

### Requirement: RQ-07 Separación recepción/documentos
El sistema SHALL cumplir EXPEDIENTE/LOTE/NO_APLICA permanecen congelados y recibir no crea expedientes ni documentos definitivos. Decisión: D-07.

#### Scenario: Barrera congelada
- **WHEN** falta un archivo en la unidad completa EXPEDIENTE o LOTE
- **THEN** no se habilitan efectos documentales y no se reduce el denominador original

#### Scenario: Recepción sin documentos
- **WHEN** se confirma y recibe un lote elegible
- **THEN** se conservan los conteos documentales y estadoEfecto NO_INICIADO de archivos elegibles

### Requirement: RQ-08 Validación por fase y capacidad
El sistema SHALL cumplir 07-A usa repositorios reales independientes de UI; RECIB-01/02/03 ejecutan 07-B con controles SELECT y evidencia saneada. Decisión: D-08.

#### Scenario: Fundaciones independientes
- **WHEN** 07-A se prueba sin ventana ni servicio de preflight ejecutable
- **THEN** las transacciones reales y SQL tienen evidencia propia y no dependen de RECIB navegador

#### Scenario: Recorrido integrado
- **WHEN** se ejecutan RECIB-01/02/03 autorizados
- **THEN** se verifican HTTP y persistencia SELECT sin invocar creación ni almacenamiento

### Requirement: RQ-09 Entrega y recuperación operativa
El sistema SHALL cumplir Cada caso identifica SHA/ambiente/estado y los no ejecutados bloquean el cierre correspondiente; SQL y datos reales requieren autorización propia. Decisión: D-09.

#### Scenario: Validación no autorizada
- **WHEN** falta autorización del ambiente/cuentas/recursos
- **THEN** los casos reales quedan NO_EJECUTADO con responsable y no se cierra la fase afectada

#### Scenario: Entrega verificable
- **WHEN** se solicita cierre
- **THEN** cada caso conserva SHA, resultado, comandos, recursos y limitaciones sin secretos
