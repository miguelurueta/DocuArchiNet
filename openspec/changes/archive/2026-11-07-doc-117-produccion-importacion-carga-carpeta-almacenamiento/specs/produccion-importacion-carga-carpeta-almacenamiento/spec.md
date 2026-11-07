## Purpose

Definir la conservación del original, la confirmación documental por identidades conocidas y la gestión de incertidumbre sin doble almacenamiento en la importación de carpetas.

## ADDED Requirements

### Requirement: RQ-01 Procedencia verificable
La entrega SHALL distinguir fuente local, resultados históricos y resultados nuevos, conservando SHA, fecha, ambiente y alcance de cada evidencia. Trazabilidad: D-01.

#### Scenario: Evidencia anterior
- **WHEN** se reutiliza un REC histórico
- **THEN** se conserva su procedencia y no se presenta como ejecución del commit actual

#### Scenario: Divergencia local
- **WHEN** la copia aislada difiere de la principal
- **THEN** se registra la diferencia sin sobrescribir la principal

### Requirement: RQ-02 Original conservado y efecto único
El sistema SHALL conservar el original recibido y usar una copia consumible privada con su nombre original; una repetición no SHALL volver a almacenar un efecto ya conocido. Trazabilidad: D-02.

#### Scenario: Copia consumida
- **WHEN** la carga consume o mueve su copia de trabajo
- **THEN** el original permanece íntegro y verificable por tamaño y hash

#### Scenario: Repetición confirmada
- **WHEN** se repite el mismo comando confirmado
- **THEN** se devuelven los mismos IDs sin nuevo almacenamiento

### Requirement: RQ-03 Valores finales ALM-01
La confirmación SHALL acreditar gabinete, expediente, documento, inventario, incorporación, tipología, nombre, extensión e índices efectivos mediante las identidades conocidas. Trazabilidad: D-03.

#### Scenario: Texto y formatos
- **WHEN** se contrastan valores finales de un formato autorizado
- **THEN** los textos y referencias conservan sus ceros; fechas son canónicas y decimales equivalen numéricamente sin normalizar texto

#### Scenario: Relación discordante
- **WHEN** falta incorporación o difiere una relación, nombre o valor esperado
- **THEN** no se confirma ni se atribuye el efecto por semejanza de nombre

### Requirement: RQ-04 Incertidumbre ALM-02
Ante un fallo posterior al efecto el sistema SHALL consultar la evidencia durable y conservar incertidumbre si no acredita confirmación; no SHALL repetir el almacenamiento para resolver una respuesta. Trazabilidad: D-04.

#### Scenario: Retorno perdido después de commit
- **WHEN** el checkpoint confirmado persiste aunque falle su retorno
- **THEN** la lectura permite devolver la identidad confirmada sin otro escritor

#### Scenario: Efecto sin checkpoint suficiente
- **WHEN** el escritor produjo un efecto pero no hay confirmación durable suficiente
- **THEN** se mantienen los IDs válidos disponibles y el resultado INCIERTO sin replay automático

### Requirement: RQ-05 Reconciliación documental acotada
La reconciliación SHALL confirmar únicamente intentos INCIERTO con identidades completas y evidencia concordante; no SHALL almacenar ni incorporar de nuevo. Trazabilidad: D-05.

#### Scenario: Prueba suficiente
- **WHEN** un intento INCIERTO tiene identidades y relaciones exactas
- **THEN** la vía soportada confirma conservando documento e inventario sin otro efecto

#### Scenario: Prueba insuficiente
- **WHEN** el intento permanece EN_CURSO, faltan IDs o falta incorporación
- **THEN** no se recrea ni repara el documento y se informa el bloqueo de confirmación

### Requirement: RQ-06 Coexistencia ALM-03 con límites
La aceptación de coexistencia SHALL distinguir solapamiento efectivo de serialización y delimitar sus conclusiones a los recursos observados. Trazabilidad: D-06.

#### Scenario: Ejecución solapada
- **WHEN** una importación y una carga individual autorizadas se solapan realmente
- **THEN** se acreditan originales intactos e identidades y relaciones propias sin duplicación, sin afirmar aislamiento XML o SII

#### Scenario: Serialización
- **WHEN** dos solicitudes quedan serializadas o no hay evidencia de solapamiento
- **THEN** la prueba no se declara PASS de concurrencia real

#### Scenario: Observación interna exclusiva de laboratorio
- **WHEN** se desarrolla la variante de observación aprobada editorialmente
- **THEN** las marcas internas solo pertenecen a una copia de laboratorio que conserva retornos/errores/ByRef y fuentes ordinarias intactas; los intervalos externos no sustituyen al método, las trazas incompletas no acreditan solapamiento y la aprobación no autoriza instalación o efectos reales

### Requirement: RQ-07 Rechazos antes del escritor
El sistema SHALL rechazar recursos no autorizados, valores incompatibles, nombres inseguros, staging alterado y formatos TIFF/Multi-TIFF antes de ejecutar almacenamiento. Trazabilidad: D-07.

#### Scenario: Recurso ajeno o alterado
- **WHEN** se aporta un campo ajeno al gabinete, tipología inválida, relación incorrecta o staging alterado
- **THEN** se rechaza el efecto sin llamar al escritor

#### Scenario: Formato o nombre rechazado
- **WHEN** se intenta TIFF/Multi-TIFF o un nombre inseguro
- **THEN** no se almacena ni amplía la lista de formatos soportados

### Requirement: RQ-08 Aceptación y autorización
La validación SHALL usar autorizaciones específicas para efectos reales y controles exclusivamente SELECT; no SHALL convertir un escenario no ejecutado en evidencia positiva. Trazabilidad: D-08.

#### Scenario: Escenario sin autorización
- **WHEN** no hay autorización del ambiente, cuentas y recursos para un caso real
- **THEN** el caso permanece NO_EJECUTADO sin efectos ni credenciales persistidas

#### Scenario: Cierre trazable
- **WHEN** se evalúa cerrar DOC-117
- **THEN** la matriz identifica casos reales y locales, límites y faltantes; no oculta escenarios no acreditados
