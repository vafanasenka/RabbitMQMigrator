namespace RabbitMQMigrator;

public enum LogType
{
    None,

    Connect,
    Connected,

    Get_Settings_Start,
    Get_Settings_Done,

    Create_Settings_Start,
    Create_Settings_Done,

    Print_Settings_Start,
    Print_Settings_Done,
    Print_Exchanges_Start,
    Print_Exchange_Done,
    Print_Queues_Start,
    Print_Queue_Done,
    Print_Bindings_Start,
    Print_Binding_Done,

    Migrate_Messages_Start,
    Migrate_Messages_Done,
    
    Done
}
