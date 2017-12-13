
¬
proto/table.protoTable"É
	CreateReq
token (	Rtoken
playoffs (Rplayoffs
times (Rtimes
jiangma (Rjiangma
maima (Rmaima""
	CreateRsp
tab_id (RtabId" 
JoinReq
tab_id (RtabId"7
Join
id (Rid
role (2.Table.RoleRrole"C
JoinRsp
err_no (RerrNo!
roles (2.Table.RoleRroles"
QuitReq
id (Rid"
QuitRsp
id (Rid"
KickReq
id (Rid"|
Role
id (Rid
name (	Rname
pos (Rpos
ready (Rready
online (Ronline
url (	Rurl"

ReadyReq"
Ready
id (Rid"E
Start
status (Rstatus
id (Rid
round (Rround"
Cards
cards (Rcards"
Turn
id (Rid"]
Play
id (Rid
card (Rcard
leftcard (Rleftcard
err_no (RerrNo"U
Peng
id (Rid
from (Rfrom
card (Rcard
err_no (RerrNo",
Angang
id (Rid
card (Rcard"U
Gang
id (Rid
from (Rfrom
card (Rcard
err_no (RerrNo"Å
Hu
id (Rid
from (Rfrom
hutype (Rhutype
err_no (RerrNo
card (Rcard
cards (Rcards"*
Pass
id (Rid
auto (Rauto"I
NewCard
card (Rcard
id (Rid
leftcard (Rleftcard"4

SwitchAuto
id (Rid
isauto (Risauto"/
	VoiceChat
id (Rid
data (Rdata" 
QuickMsg
msgid (Rmsgid"
JiangMa
cards (Rcards"2

RoundScore$
scores (2.Table.ScoreRscores"V
Score
id (Rid
hu (Rhu
hu_type (RhuType
score (Rscore