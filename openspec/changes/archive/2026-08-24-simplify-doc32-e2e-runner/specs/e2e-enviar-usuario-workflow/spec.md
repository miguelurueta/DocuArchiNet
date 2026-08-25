## MODIFIED Requirements

### Requirement: Evidencia libre de secretos y cierre seguro

La automatización SHALL recibir credenciales y conexiones solo mediante variables efímeras. Podrá reutilizar un perfil externo únicamente para campos no sensibles validados por el contrato de su DOC; SHALL rechazar perfiles que incluyan secretos, cookies, cadenas de conexión o autorizaciones. SHALL generar evidencia resumida que no contenga secretos, cookies, cadenas de conexión ni cuerpos de respuesta completos.

#### Scenario: Perfil no sensible reutilizable

- **WHEN** una corrida de envío a usuario recibe un perfil externo con solo los campos permitidos
- **THEN** reutiliza esos campos sin persistir credenciales ni autorizaciones y solicita los secretos efímeros de forma segura.

#### Scenario: Cierre de una corrida autorizada

- **WHEN** termina una corrida de preview o ejecución autorizada
- **THEN** se conserva únicamente resultado, códigos, conteos y huellas, y se verifica que el gate permanezca apagado y sus listas vacías.
