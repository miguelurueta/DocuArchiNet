## MODIFIED Requirements

### Requirement: Comportamiento de control por tipo de campo
El sistema SHALL reaccionar al cambio del campo `data-ident="pl-radicacion-spe-Descripcion_Documento"` tomando el `idValue` seleccionado para consumir la API `/api/tramite/tramites/empsolicitaListaflujosRelacionadosTramite` con el parametro `idTipoDocEntrante`. Con la respuesta, SHALL poblar o limpiar el campo `data-ident="pl-radicacion-spe-RE_flujo_trabajo"` sin romper atributos declarativos existentes (`required`, `disabled`, `title`, `tooltipAyuda`) y con manejo de errores centralizado en la capa axios/hook reutilizable.

#### Scenario: Consulta flujos relacionados al seleccionar tramite valido
- **WHEN** el usuario selecciona un tramite y su `idValue` es valido
- **THEN** el sistema llama la API de flujos relacionados enviando `idTipoDocEntrante` con ese `idValue`

#### Scenario: No consulta y limpia flujos cuando idValue es null
- **WHEN** el `idValue` del tramite es `null` o no existe seleccion valida
- **THEN** el sistema no llama la API y limpia los items del campo `pl-radicacion-spe-RE_flujo_trabajo`

#### Scenario: Llena flujos con respuesta exitosa
- **WHEN** la API responde `success=true` con elementos en `data` (`idValue`, `Value`)
- **THEN** el selector `pl-radicacion-spe-RE_flujo_trabajo` actualiza sus opciones con esos elementos

#### Scenario: Limpia flujos cuando la API retorna vacio o error controlado
- **WHEN** la API retorna `data` vacio o ocurre error en consumo
- **THEN** el sistema limpia las opciones de flujo y maneja el error mediante la capa centralizada sin romper el formulario
