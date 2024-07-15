namespace RabbitMQMigrator;

public enum LogType
{
    None,

    Connect,
    Connected,

    Get_Components_Start,
    Get_Components_Done,

    Create_Settings_Start,
    Create_Settings_Done,

    // log for debug & test
    Log_Components_Start,
    Log_Components_Done,
    Log_Exchanges_Start,
    Log_Exchange_Done,
    Log_Queues_Start,
    Log_Queue_Done,
    Log_Bindings_Start,
    Log_Binding_Done,

    Migrate_Messages_Start,
    Migrate_Messages_Done,
    
    Done
}
