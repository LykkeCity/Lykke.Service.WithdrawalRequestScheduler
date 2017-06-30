# README #

This Job monitors CashOut requests with status ClientConfirmation.
If such request is expired (it's "DateTime" property less thes some expiration date) then request will be declined with status CanceledByTimeout.

For decline requests WalletAPI's method "api/CashOutSwiftRequest/CancelRequestsByTimeout" used
