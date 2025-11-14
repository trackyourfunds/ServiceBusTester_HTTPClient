using System;
using System.Collections.Generic;

public class Payout
{
 public int amount { get; set; }
 public string provider_account_id { get; set; }
 public string payout_id { get; set; }
 public string previous_transfer_id { get; set; }
 public string synced_at { get; set; } // ISO8601 string
 public string transacted_at { get; set; } // ISO8601 string
 public string cart_id { get; set; }
 public string cart_item_id { get; set; }
 public string store_item_id { get; set; }
 public string payment_method { get; set; }
 public int fee { get; set; }
 public int total { get; set; }
 public int total_to_cause { get; set; }
 public int quantity { get; set; }
 public string processing_fees { get; set; }
 public string item_type { get; set; }
 public bool shop_to_give { get; set; }
 public bool pos { get; set; }
 public bool product_fundraiser { get; set; }
 public int price { get; set; }
 public string account_code { get; set; }
 public string account_code_id { get; set; }
 public string account_code_name { get; set; }
 public string cause_id { get; set; }
 public bool transfer { get; set; }
 public bool reversal { get; set; }
 public string uuid { get; set; }
 public Cause cause { get; set; }
 public string provider_created_at { get; set; } // ISO8601 string
 public string created_at { get; set; } // ISO8601 string
 public string updated_at { get; set; } // ISO8601 string
 public object payout { get; set; }
 public StoreItem store_item { get; set; }
 public int fee_passed_on { get; set; }
 public int fee_absorbed { get; set; }
 public int fee_obligation { get; set; }
}

public class Cause
{
 public string name { get; set; }
 public string unit_type { get; set; }
 public string provider_account_id { get; set; }
 public string uuid { get; set; }
 public string created_at { get; set; } // ISO8601 string
 public string updated_at { get; set; } // ISO8601 string
}

public class StoreItem
{
 public string name { get; set; }
 public string cause_id { get; set; }
 public List<Category> categories { get; set; }
 public int price { get; set; }
 public string type { get; set; }
 public string uuid { get; set; }
 public string created_at { get; set; } // ISO8601 string
 public string updated_at { get; set; } // ISO8601 string
}

public class Category
{
 public string name { get; set; }
 public string uuid { get; set; }
}
