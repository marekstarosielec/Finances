/**
 * FinancesApi
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


export interface Transaction { 
    scrappingDate?: string | null;
    status?: string | null;
    id?: string | null;
    source?: string | null;
    date?: string;
    account?: string | null;
    category?: string | null;
    amount?: number;
    title?: string | null;
    description?: string | null;
    text?: string | null;
    readonly bankInfo?: string | null;
    comment?: string | null;
    currency?: string | null;
    details?: string | null;
    person?: string | null;
    caseName?: string | null;
    settlement?: string | null;
}

