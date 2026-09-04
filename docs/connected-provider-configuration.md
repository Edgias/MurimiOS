# Connected-provider configuration

MurimiOS runs connected-agriculture ingestion in the worker process. Provider credentials are stored through the existing protected credential-reference service; do not put passwords, API keys, or bearer tokens in connection or feed configuration JSON.

## Generic MQTT sensors

Register an active `Sensor` connection with provider code `mqtt` and connection configuration such as:

```json
{
  "host": "mqtt.example.net",
  "port": 8883,
  "tls": true,
  "clientIdPrefix": "murimios",
  "keepAliveSeconds": 30,
  "sessionExpirySeconds": 86400
}
```

Store the protected credential payload as `{"username":"device-reader","password":"..."}`. Register an active `Sensor` feed for the target farm and device:

```json
{
  "topic": "farms/+/sensors/+",
  "qos": 1,
  "externalDeviceIdProperty": "deviceId",
  "observedAtProperty": "observedAt",
  "sequenceProperty": "sequence",
  "valuesProperty": "values",
  "units": {
    "soilMoisture": "%",
    "temperature": "C"
  }
}
```

Each message must be a JSON object. By default, `values` is an object whose numeric properties become sensor-channel readings. Property names can be changed in the feed configuration. The worker maintains a durable MQTT 5 session, reconnects with bounded backoff, and deduplicates messages before appending immutable readings.

## The Things Stack LoRaWAN

Register a `Sensor` connection with provider code `the-things-stack`. Use the cluster MQTT hostname, TLS port `8883`, and a protected credential containing the application username and API key:

```json
{
  "username": "my-application@my-tenant",
  "apiKey": "..."
}
```

Configure a feed topic such as `v3/my-application@my-tenant/devices/+/up`. The worker enforces MQTT 3.1.1, clean sessions, and QoS 0 for this provider. It reads device identity, frame counter, received time, decoded or normalized payload, firmware version, and gateway/location metadata from The Things Stack uplink envelope. Unit mappings use the same `units` object as generic sensor feeds.

## MQTT machinery telemetry

Register a `Machinery` connection with provider code `mqtt` and a `Machinery` feed. The feed uses the MQTT connection properties above and supports mappings for `externalEquipmentIdProperty`, `externalDeviceIdProperty`, `observedAtProperty`, `latitudeProperty`, `longitudeProperty`, `speedProperty`, `engineHoursProperty`, `fuelUsedProperty`, `workedAreaProperty`, `taskCodeProperty`, and `attributesProperty`. Normalized records are appended to the machinery telemetry store and linked to the farm's equipment and connected-device registries.

## Canonical machinery REST

Register a `Machinery` connection with provider code `canonical-rest`. Connection configuration supplies the HTTPS base URL and optional timeout; the feed supplies the relative endpoint:

```json
{
  "endpoint": "/v1/telemetry",
  "pageSize": 500
}
```

Credentials may use `{"scheme":"Bearer","token":"..."}` or `{"header":"X-Api-Key","value":"..."}`. The endpoint returns either a JSON array or an object with `records`, optional `nextCursor`, and optional `retryAfterSeconds`. Each record uses the canonical machinery telemetry property names described above. Cursor checkpoints, provider rate limits, raw-payload provenance, retries, deduplication, and dead lettering use the shared ingestion infrastructure.

## Canonical ERP documents

Register an `Erp` connection with provider code `canonical-rest`. A record contains `externalDocumentId`, `documentType`, `occurredAt`, optional `currency`, `totalAmount`, `counterparty`, `status`, and arbitrary `payload`. Documents are stored as inbound business-document exchanges with raw provenance and an idempotent external identifier.

## Canonical accounting statements

Register an `Accounting` connection with provider code `canonical-rest`. Each record contains `externalBatchId`, `periodStart`, `periodEnd`, `currency`, and `lines`. A line contains `externalLineId`, `occurredAt`, `accountCode`, `description`, `debit`, `credit`, and optional reconciliation dimensions such as farm, field, crop zone, livestock subject, and external document reference. The worker creates accounting import batches and immutable statement lines for the existing reconciliation workflow.

## Operations and security

- The API runtime and worker use the least-privilege application database role. Cross-tenant MQTT discovery returns only opaque IDs and version stamps; each full configuration is subsequently loaded under that tenant's row-level-security context.
- Updating a connection, feed, or credential version restarts its subscription automatically. Disabling any of them removes the live subscription during discovery.
- TLS defaults to enabled. Plaintext MQTT must be explicitly configured and should be limited to isolated development networks.
- Health, ingestion jobs, attempts, feed freshness, raw records, outbox events, and dead letters provide the operational audit trail. Use replay only after correcting a permanent mapping or provider-data problem.
- Provider-specific production onboarding still requires endpoint contracts, credentials, data-processing terms, representative payload fixtures, and a live smoke test in the target provider account.
