# In-room player-spawning Peocedure

## Introduce

### 1. Client queries and uploads player data to Host/Server

```mermaid
sequenceDiagram
    autonumber
    box Client
      participant UserService
      participant CltBroker as PlayerDataBroker
      participant NetworkRunner
    end
    
    box Host/Server
      participant SvrBroker as PlayerDataBroker
    end
    
    NetworkRunner->>CltBroker: Spawned()
    CltBroker->>NetworkRunner: localPlayer = GetLocalPlayer()
    alt localPlayer != PlayerRef.None
      CltBroker-->>+UserService: QueryEntityData<User>(OnResult)
      UserService-->>-CltBroker: OnQueryResult(User)
      CltBroker-->>SvrBroker: SendPlayerDataRPC(netPlayerData, playerRef)
    end
```

### 2. Server/Host receives player data from client and spawns player in room

```mermaid
sequenceDiagram
    autonumber
    box Client
      participant CltBroker as PlayerDataBroker
    end
    box Host/Server
      participant SvrBroker as PlayerDataBroker
      participant PlayerSpawner
      participant SvrPlayerService as PlayerService
      participant SvrNetRunner as NetworkRunner
      participant SvrPlayer as Player
    end

    CltBroker-->>SvrBroker: SendPlayerDataRPC(netPlayerData, playerRef)
    SvrBroker-->>PlayerSpawner: EntityData(playerRef, netPlayerData)
    PlayerSpawner->>SvrNetRunner: Spawn(PlayerPrefab, playerRef)
    SvrNetRunner->>SvrPlayer: Instantiate
    PlayerSpawner->>SvrPlayer: Init(netPlayerData)
    SvrNetRunner->>+SvrPlayer: AfterSpawned()
    SvrPlayer->>SvrPlayerService: Register(SvrPlayer)
    SvrPlayer->>-SvrPlayer: LoadAvatar()
```

### 3. The newly-created player is synchronized with and spawned on clients

```mermaid
sequenceDiagram
    autonumber
    box Client
      participant CltPlayerService as PlayerService
      participant CltNetRunner as NetworkRunner
      participant CltPlayer as Player
    end
    box Host/Server
      participant SvrNetRunner as NetworkRunner
    end

    SvrNetRunner-->>CltNetRunner: State Sync
    CltNetRunner->>CltPlayer: Instantiate
    CltNetRunner->>+CltPlayer: AfterSpawned()
    CltPlayer->>CltPlayerService: Register(CltPlayer)
    CltPlayer->>-CltPlayer: LoadAvatar()
```
