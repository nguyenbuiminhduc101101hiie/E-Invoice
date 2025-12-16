-- Query test để select paymentDate từ PaymentHistory (JSON array)
-- PaymentHistory có dạng: [{"cashAmount":0,"transferAmount":1160000,"paymentDate":"2025-12-11T16:38:37.875"}]

-- Cách 1: Sử dụng OPENJSON với CROSS APPLY
SELECT 
    o.id,
    o.CustomerName,
    o.CustomerPhone,
    p.paymentDate,
    CAST(p.paymentDate AS date) AS paymentDateOnly
FROM Orders_viettel o
CROSS APPLY OPENJSON(o.PaymentHistory)
WITH (
    paymentDate datetime2 '$.paymentDate'
) AS p
WHERE p.paymentDate IS NOT NULL
ORDER BY p.paymentDate DESC;

-- Cách 2: Sử dụng OPENJSON không có WITH clause (lấy toàn bộ JSON)
SELECT 
    o.id,
    o.CustomerName,
    o.CustomerPhone,
    j.[key] AS arrayIndex,
    j.value AS paymentRecord,
    JSON_VALUE(j.value, '$.paymentDate') AS paymentDate,
    JSON_VALUE(j.value, '$.cashAmount') AS cashAmount,
    JSON_VALUE(j.value, '$.transferAmount') AS transferAmount
FROM Orders_viettel o
CROSS APPLY OPENJSON(o.PaymentHistory) AS j
WHERE JSON_VALUE(j.value, '$.paymentDate') IS NOT NULL
ORDER BY JSON_VALUE(j.value, '$.paymentDate') DESC;

-- Cách 3: Filter theo paymentDate trong khoảng thời gian
SELECT 
    o.id,
    o.CustomerName,
    o.CustomerPhone,
    p.paymentDate,
    CAST(p.paymentDate AS date) AS paymentDateOnly
FROM Orders_viettel o
CROSS APPLY OPENJSON(o.PaymentHistory)
WITH (
    paymentDate datetime2 '$.paymentDate'
) AS p
WHERE p.paymentDate IS NOT NULL
AND CAST(p.paymentDate AS date) BETWEEN '2025-12-01' AND '2025-12-31'
ORDER BY p.paymentDate DESC;

-- Cách 4: Lấy paymentDate đầu tiên trong mảng (nếu có nhiều records)
SELECT 
    o.id,
    o.CustomerName,
    o.CustomerPhone,
    (
        SELECT TOP 1 JSON_VALUE(j.value, '$.paymentDate')
        FROM OPENJSON(o.PaymentHistory) AS j
        WHERE JSON_VALUE(j.value, '$.paymentDate') IS NOT NULL
        ORDER BY JSON_VALUE(j.value, '$.paymentDate') DESC
    ) AS latestPaymentDate
FROM Orders_viettel o
WHERE o.PaymentHistory IS NOT NULL
AND o.PaymentHistory != '[]'
AND o.PaymentHistory != '';

