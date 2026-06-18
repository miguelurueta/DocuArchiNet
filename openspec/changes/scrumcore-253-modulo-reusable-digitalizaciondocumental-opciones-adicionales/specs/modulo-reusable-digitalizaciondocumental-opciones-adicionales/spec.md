## ADDED Requirements

### Requirement: Procesamiento automatico de imagenes documentales
El sistema SHALL permitir configurar procesamiento automatico de calidad documental en el digitalizador reutilizable.

#### Scenario: Opciones desactivadas por defecto
- **WHEN** se abre el digitalizador
- **THEN** Deskew, Auto Crop y Auto Rotate aparecen desactivados por defecto.

#### Scenario: Persistencia durante la sesion
- **WHEN** el usuario activa una opcion de procesamiento automatico
- **THEN** la opcion permanece activa durante la sesion actual del digitalizador.

### Requirement: Auditoria de capacidades Dynamsoft
El sistema SHALL auditar capacidades reales del runtime Dynamsoft antes de activar procesamiento automatico.

#### Scenario: Capacidad soportada
- **WHEN** el runtime expone una API compatible para Deskew, Auto Crop o Auto Rotate
- **THEN** el adapter puede invocarla desde el flujo de procesamiento.

#### Scenario: Capacidad no soportada
- **WHEN** el runtime no expone una API compatible
- **THEN** la funcionalidad queda documentada como no disponible o se maneja con error controlado sin romper escaneo.

### Requirement: Deskew
El sistema SHALL corregir inclinacion de paginas si la capacidad esta soportada.

#### Scenario: Pagina inclinada
- **WHEN** una pagina capturada esta inclinada y Deskew esta activo
- **THEN** el sistema corrige la inclinacion y actualiza miniatura, preview y PDF final.

### Requirement: Auto Crop
El sistema SHALL recortar bordes externos si la capacidad esta soportada.

#### Scenario: Bordes o margenes vacios
- **WHEN** una pagina contiene bordes negros o margenes vacios y Auto Crop esta activo
- **THEN** el sistema recorta el area externa sin eliminar contenido documental.

### Requirement: Auto Rotate
El sistema SHALL corregir orientacion de paginas si la capacidad esta soportada.

#### Scenario: Pagina girada
- **WHEN** una pagina esta girada 90 o 180 grados y Auto Rotate esta activo
- **THEN** el sistema corrige la orientacion y actualiza miniatura, preview y PDF final.

### Requirement: No regresion de flujo documental
El sistema SHALL conservar los comportamientos existentes del digitalizador.

#### Scenario: Funciones existentes
- **WHEN** se activan o desactivan opciones de procesamiento automatico
- **THEN** escaneo simplex, duplex, ADF, blank page removal, Drag & Drop, rotacion manual y generacion PDF siguen funcionando.

### Requirement: Rendimiento medible
El sistema SHALL medir tiempos de procesamiento automatico.

#### Scenario: Metricas por capacidad
- **WHEN** se ejecuta Deskew, Auto Crop o Auto Rotate
- **THEN** se emiten metricas compactas `DESKEW_TIME`, `AUTOCROP_TIME` o `AUTOROTATE_TIME`.
