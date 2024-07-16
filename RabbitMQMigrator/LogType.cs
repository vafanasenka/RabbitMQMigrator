namespace RabbitMQMigrator;

public enum LogType
{
    None,

    Connect,
    Connected,

    Get_Components_Start,
    Get_Components_Done,

    Delete_Settings_Start,
    Delete_Settings_Done,

    Create_Settings_Start,
    Create_Settings_Done,

    // log for debug & test
    Log_Components_Start,
    Log_Components_Done,

    Log_Exchanges_Start,
    Log_Exchanges_Done,
    Log_Exchange_Done,
    
    Log_Queues_Start,
    Log_Queues_Done,
    Log_Queue_Done,
    
    Log_Bindings_Start,
    Log_Bindings_Done,
    Log_Binding_Done,

    Migrate_Exchanges_Start,
    Migrate_Exchanges_Done,
    
    Migrate_Queues_Start,
    Migrate_Queues_Done,

    Migrate_Bindings_Start,
    Migrate_Bindings_Done,

    Migrate_Messages_Start,
    Migrate_Messages_Done,

    Delete_Exchanges_Start,
    Delete_Exchanges_Done,

    Delete_Queues_Start,
    Delete_Queues_Done,

    Delete_Bindings_Start,
    Delete_Bindings_Done,

    Delete_Messages_Start,
    Delete_Messages_Done,

    Error,
    Exception,
    Done
}
