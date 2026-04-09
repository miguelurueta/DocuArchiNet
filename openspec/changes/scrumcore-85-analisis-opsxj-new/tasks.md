## 1. Levantamiento del flujo actual

- [ ] 1.1 Revisar como `opsxj:new` obtiene datos de Jira y delega la construccion de `proposal.md`
- [ ] 1.2 Identificar en que capa se transforma `summary` y `description` en capability, impacto y texto base del proposal

## 2. Evidencia del problema

- [ ] 2.1 Documentar ejemplos del repo donde el proposal generado queda generico o con capability artificial
- [ ] 2.2 Relacionar esos ejemplos con pruebas, documentacion o cambios archivados que ya expresan el comportamiento esperado

## 3. Delimitacion tecnica

- [ ] 3.1 Separar en el diagnostico los problemas de orquestacion `opsxj:new` de los problemas de inferencia del generador de proposals
- [ ] 3.2 Identificar los archivos, pruebas y documentos que tendria que tocar un ticket posterior de correccion

## 4. Salida del analisis

- [ ] 4.1 Consolidar recomendaciones concretas para un ticket posterior sin aplicar cambios funcionales en este ticket
- [ ] 4.2 Cerrar el cambio dejando explicito que el resultado es diagnostico y no implementacion
